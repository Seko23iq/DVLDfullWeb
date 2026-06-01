import {formatDate, showToast, GetPersonIDbyLicenseID} from "./Global.js";
import {searchLicense } from "./searchLicense.js";
import {getApplicationTypeFeesByApplicationID } from "./ManageApplicationTypeFees.js";
import {fetchLicenseClassInfo } from "./LicenseClassInfo.js";



document.addEventListener("DOMContentLoaded", async () => {
    document.querySelector(".ApplicationDate").textContent = formatDate(Date.now());
    document.querySelector(".renewIssueDate").textContent = formatDate(Date.now());

    const userName = localStorage.getItem("UserName");
    document.querySelector(".createdBy").textContent = userName;

    document.querySelector(".fees-value").textContent =  await getApplicationTypeFeesByApplicationID(2);

    document.querySelector(".btn-issue").disabled = true;
    document.querySelector(".ShowLicenseInfo").classList.add("disabled");
    document.getElementById("btnIssue").addEventListener("click", issueApplication);
});

let PersonID = 0;

const btnSearch = document.getElementById("btnSearch");
btnSearch.addEventListener("click", async () => {
    await searchLicense();
    await ShowOldLicenseID();

    IsLicenseExpirate();

    await GetPersonIDfromLicenseID();
    await GetLicenseClassFeesAndDefaultValidityLength();
});

function ShowOldLicenseID()
{
    const licenseIdInput = document.getElementById("licenseIdInput");
    const licenseId = licenseIdInput.value.trim();
    
    if (!licenseId) {
        return;
    }
    document.querySelector(".OldLicenseID").textContent = licenseId;
   

}
function IsLicenseExpirate()
{
    const expirationDate = document.querySelector(".ExpirationDate").textContent;
    const DateNow = formatDate(Date.now());

    if(expirationDate > DateNow)
    {
        showToast(`Selected License is not yes expiared, it will expire on: ${expirationDate}`, "error");
        document.querySelector(".btn-issue").disabled = true;
        document.querySelector(".ShowLicenseInfo").classList.add("disabled");
        return;
    }
}
async function GetPersonIDfromLicenseID()
{
    const LicenseID = document.querySelector(".LicenseID").textContent; 
    
    if(LicenseID > 0)
    {
        PersonID = await GetPersonIDbyLicenseID(Number(LicenseID));
    }
}
let LicenseClassInfo;
async function GetLicenseClassFeesAndDefaultValidityLength()
{
    const ClassName = document.querySelector(".Class").textContent;
    LicenseClassInfo = await fetchLicenseClassInfo(ClassName);

    document.querySelector(".licenseFees").textContent = LicenseClassInfo.classFees;

    document.querySelector(".totalFees").textContent =
        Number(document.querySelector(".fees-value").textContent) +
        Number(document.querySelector(".licenseFees").textContent);


    document.querySelector(".renewExpirationDate").textContent = formatDate(
        new Date(new Date().setFullYear(new Date().getFullYear() + LicenseClassInfo.defaultValidityLength))
    );

}


async function issueApplication() {
    const userID = Number(localStorage.getItem("UserID")) || 0;

    const OldLicenseID     = Number(document.querySelector(".OldLicenseID")?.textContent.trim()  || 0);
    const driverID     = Number(document.querySelector(".DriverID")?.textContent.trim()   || 0);
    const licenseClass = Number(document.querySelector(".Class")?.dataset?.classId        || 0);
    const oldLicenseID = Number(document.querySelector(".OldLicenseID")?.textContent.trim() || 0);
    const notes        = document.getElementById("Notes").value.trim();
    const appFees      = Number(document.querySelector(".applicationFees")?.textContent   || 7);
    const licFees      = Number(document.querySelector(".licenseFees")?.textContent       || 7);

    const now          = new Date();
    const expiration   = new Date(new Date().setFullYear(now.getFullYear() + LicenseClassInfo.defaultValidityLength));

const dto = {
    OldLicenseID: OldLicenseID,
    application: {
        applicantPersonID: PersonID,
        applicationDate:   now.toISOString(),
        applicationTypeID: 2,
        applicationStatus: 3, // Complete
        paidFees:          appFees,
        createdByUserID:   userID
    },
    license: {
        driverID:          Number(document.querySelector(".DriverID")?.textContent.trim() || 0),
        licenseClass:      LicenseClassInfo.licenseClassID,
        issueDate:         now.toISOString(),
        expirationDate:    expiration.toISOString(),
        notes:             notes,
        paidFees:          licFees,
        isActive:          true,
        issueReason:       2,
        createdByUserID:   userID
    }
};

    console.log(dto);

    document.getElementById("btnIssue").disabled = true;
    await addRenewLicense(dto);
}

async function addRenewLicense(dto) {
    const btnIssue = document.getElementById("btnIssue");

try {
        btnIssue.disabled = true;
        const response = await fetch("https://localhost:7223/api/RenewLicense/AddRenewLicense", {
            method:  "POST",
            headers: { "Content-Type": "application/json" },
            body:    JSON.stringify(dto)
        });
        if (!response.ok) {
            const errText = await response.text();
            throw new Error(errText || response.statusText);
        }
        const result = await response.json();
        // التحقق من وجود البيانات المرجعة
        if (!result?.applicationID || !result?.licenseID) {
            throw new Error("Invalid response: missing IDs.");
        }
        // ── تحديث الـ UI بعد النجاح ──────────────────────────
        document.querySelector(".RLApplicationID").textContent  = result.applicationID;
        document.querySelector(".RenewedLicenseID").textContent = result.licenseID;
        document.querySelector(".ShowLicenseInfo").classList.remove("disabled");
        showToast("License renewed successfully!", "success");
    } catch (err) {
        console.error("Renew license failed:", err);
        showToast(`Failed to renew license: ${err.message}`, "error");
        btnIssue.disabled = false; // إعادة تفعيل الزر فقط عند الخطأ
    }
}

function closeApplication() {
    history.back();
}

const btnCancel = document.getElementById("btnCancel");
btnCancel.addEventListener("click", () => {
    history.back();
});

const ShowLicenseHistory = document.getElementById("ShowLicenseHistory");
ShowLicenseHistory.addEventListener("click", () => {
    window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
});

const ShowLicenseInfo = document.getElementById("showLicenseInfo");
ShowLicenseInfo.addEventListener("click", () => {
    const RenewedLicenseID = document.querySelector(".RenewedLicenseID").textContent; 
    window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${RenewedLicenseID}`;
});

