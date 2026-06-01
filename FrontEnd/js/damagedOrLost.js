import {formatDate, showError, showToast,GetPersonIDbyLicenseID} from "./Global.js";
import {searchLicense} from "./searchLicense.js";
import {getApplicationTypeFeesByApplicationID} from "./ManageApplicationTypeFees.js";

const btnSearch = document.getElementById("btn-search");
const btnIssue = document.getElementById("btnIssue");

const API_Damage_Base = "https://localhost:7223/api/DamageApplication/AddDamageLicense";
const API_Lost_Base = "https://localhost:7223/api/LostApplication/AddLostLicense";

document.addEventListener("DOMContentLoaded", () => {
    document.querySelector(".ApplicationDate").textContent = formatDate(Date.now());
    const userName = localStorage.getItem("UserName");
    document.querySelector(".CreatedBy").textContent = userName;

    document.querySelector(".fees-value").textContent = 7;

    document.getElementById("btnIssue").disabled = true;
    document.querySelector(".ShowLicenseInfo").classList.add("disabled");
    document.querySelector(".ShowLicenseHistory").classList.add("disabled");

});

let PersonID = 0;
btnSearch.addEventListener("click", async () => {
    const LicenseID = document.getElementById("licenseIdInput").value;

    if (LicenseID) {
        await searchLicense();
        
        document.getElementById("btnIssue").disabled = false;
        
        PersonID = await GetPersonIDbyLicenseID(LicenseID);
        
        CheckLicenseIsActive();
    }
});

function CheckLicenseIsActive()
{
    const isActive = document.querySelector(".status-badge").textContent;

    if (isActive !== "Active") {
        showToast("Selected License is not Active, choose an active license.", "error");
        document.getElementById("btnIssue").disabled = true;
    }
}

btnIssue.addEventListener("click", () => {
    issueApplication();
})
function getAPIBase() {
    const type = document.querySelector('input[name="replacementType"]:checked')?.value;
    return type === 'lost' ? API_Lost_Base : API_Damage_Base;
}

function getDTO({ personID, userID, driverID, licenseClass, now, expiration, notes, licFees }) {
    const type = document.querySelector('input[name="replacementType"]:checked')?.value;

    const OldLicenseID = document.querySelector(".OldLicenseID").textContent;
    const applicationData = {
        applicantPersonID: personID || 1,
        createdByUserID:   userID   || 1
    };

    const licenseData = {
        driverID:       driverID     || 1,
        licenseClass:   licenseClass || 3,
        issueDate:      now.toISOString(),
        expirationDate: expiration.toISOString(),
        notes:          notes,
        paidFees:       licFees,
        isActive:       true,
        issueReason:    type === 'damaged' ? 3 : 4,
    };

    console.log(Number(OldLicenseID), applicationData, licenseData);

    return type === 'lost'
        ? { OldLicenseID: Number(OldLicenseID), lostApplication: applicationData,   lostLicense:    licenseData }
        : { OldLicenseID: Number(OldLicenseID), DamageApplication: applicationData, damagedLicense: licenseData };
}

let newLicenseID = 0;
async function issueApplication() {

    const API_Base = getAPIBase(); // ← دايماً صحيحة

    const userID = Number(localStorage.getItem("UserID")) || 0;

    // ── جمع البيانات من الـ UI ─────────────────────────────────
    const personID     = Number(document.querySelector(".NationalNo")?.dataset?.personId  || 0);
    const driverID     = Number(document.querySelector(".DriverID")?.textContent.trim()   || 0);
    const licenseClass = Number(document.querySelector(".Class")?.dataset?.classId        || 0);
    const oldLicenseID = Number(document.querySelector(".OldLicenseID")?.textContent.trim() || 0);
    const notes        = document.getElementById("Notes").textContent.trim();
    const appFees      = Number(document.querySelector(".applicationFees")?.textContent   || 7);
    const licFees      = Number(document.querySelector(".licenseFees")?.textContent       || 7);

    const now          = new Date();
    const expiration   = new Date(new Date().setFullYear(now.getFullYear() + 1));

// ── بناء الـ DTO ───────────────────────────────────────────
    const dto      = getDTO({ personID, userID, driverID, licenseClass, now, expiration, notes, licFees });

    console.log(dto);
    try {
        const response = await fetch(API_Base, {
            method:  "POST",
            headers: { "Content-Type": "application/json" },
            body:    JSON.stringify(dto)
        });

        if (!response.ok) {
            const errText = await response.text();
            throw new Error(errText || response.statusText);
        }


        const result = await response.json();

        console.log(result);

        document.querySelector(".LRApplicationID").textContent  = result.applicationID  ?? "-";
        document.querySelector(".ReplacedLicenseID").textContent = result.replacedLicenseID      ?? "-";

        newLicenseID = result.replacedLicenseID;
        showToast("License renewed successfully!", "success");
        
        document.getElementById("btnIssue").disabled = true;
        document.querySelector(".ShowLicenseInfo").classList.remove("disabled");
        document.querySelectorAll("input").forEach(input => input.disabled = true);


    } catch (err) {
        console.error("Issue failed:", err);
        showToast(`Failed to issue license: ${err.message}`, "error");
    }
}


// ApplicationFees
const lostType = document.getElementById("lostType");
lostType.addEventListener("click",  () => {
    ChangeTitleAndApplicationFees("lost",3);
})
const replacementType = document.getElementById("replacementType");
replacementType.addEventListener("click",  () => {
    ChangeTitleAndApplicationFees("damaged",4);
})

async function ChangeTitleAndApplicationFees(ApplicationType, ApplicationID)
{
    updateReplacementType(ApplicationType);

    const damagedApplicationFees = await getApplicationTypeFeesByApplicationID(ApplicationID);
    document.querySelector(".ApplicationFees").textContent = damagedApplicationFees;

}

function updateReplacementType(value) {
    const title = document.getElementById('pageTitle');
    if (value === 'damaged') {
        title.textContent = 'Replacement for Damaged License';
    } else {
        title.textContent = 'Replacement for Lost License';
    }
}



const showLicenseHistory = document.getElementById("showLicenseHistory");
showLicenseHistory.addEventListener("click", () => {
    window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
})


const showLicenseInfo = document.getElementById("showLicenseInfo");
showLicenseInfo.addEventListener("click", () => {
    window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${newLicenseID}`;
})