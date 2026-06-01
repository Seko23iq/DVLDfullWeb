import {formatDate, showError, showToast,GetPersonIDbyLicenseID} from "./Global.js";
import {searchLicense} from "./searchLicense.js";
import {getApplicationTypeFeesByApplicationID} from "./ManageApplicationTypeFees.js";

function getLicenseIDFromURL() {
    return new URLSearchParams(window.location.search).get("licenseId");
}

const btnSearch = document.getElementById("btn-search");
const licenseIdInput = document.getElementById("licenseIdInput");
const btnRelease = document.getElementById("btnRelease");


document.addEventListener("DOMContentLoaded", async () => {
    const licenseIdInput = document.getElementById("licenseIdInput");
    document.getElementById("btnRelease").disabled = true;

    if (!licenseIdInput.value) {
        document.querySelector(".ShowLicenseInfo").classList.add("disabled");
        document.querySelector(".showLicenseHistory").classList.add("disabled");
    } else {
        LicenseID = licenseIdInput.value;
        PersonID = await GetPersonIDbyLicenseID(getLicenseIDFromURL());
    }

    document.querySelector(".ApplicationFees").textContent = 
        await getApplicationTypeFeesByApplicationID(5);
    await LoadLicenseFromURL();
});

let LicenseID = "";
let PersonID = 0;

btnSearch.addEventListener("click", async () => {
    const searchLicenseResult = await searchLicense();
    if(searchLicenseResult === undefined)
    {
        return;
    }
    LicenseID = licenseIdInput.value;
    PersonID = await GetPersonIDbyLicenseID(LicenseID);
    
    const IsDetained = document.querySelector(".IsDetained .status-badge").textContent;

    if(IsDetained === "Not Detained")
    {
        showToast("Your license is not detained!", "error");
        btnRelease.disabled = true;
        return;
    }

    await GetDetainInfo();
    
    document.getElementById("btnRelease").disabled = false;

    document.querySelector(".DetainLicenseID").textContent = LicenseID;

    document.querySelector(".ShowLicenseInfo").classList.remove("disabled");
    document.querySelector(".showLicenseHistory").classList.remove("disabled");

})



async function GetDetainInfo() {

    const API_BASE = `https://localhost:7223/api/DetainReleaseLicense/GetDetainInfo?LicenseID=${LicenseID}`;

    try {
        const response = await fetch(API_BASE);

        if(!response.ok)
        {
            const errText = await response.text();
            throw new Error(errText || response.statusText);
        }

        const result = await response.json();

        document.querySelector(".DetainID").textContent = result.detainID
        document.querySelector(".DetainDate").textContent = formatDate(result.detainDate);
        document.querySelector(".CreatedBy").textContent = result.userName;
        document.querySelector(".FineFees").textContent = result.fineFees;

        document.querySelector(".TotalFees").textContent =  Number(document.querySelector(".ApplicationFees").textContent) + Number(result.fineFees);


    } catch (err) {
        console.error("Issue failed:", err);
        showToast(`Failed to get detain license info : ${err.message}`, "error");
    }
}

btnRelease.addEventListener("click", async () => {
    await ReleaseLicense();
})
async function ReleaseLicense() {

    const API_BASE = `https://localhost:7223/api/DetainReleaseLicense/ReleaseLicense`;



    const LicenseID = document.querySelector(".DetainLicenseID").textContent;
    const detainID = document.querySelector(".DetainID").textContent;
    const createdByUserID = localStorage.getItem("UserID");


    const dto = {
        "releaseDetainedApplication": {
            "applicantPersonID": PersonID,
            "createdByUserID": createdByUserID
        },
        "licenseID": LicenseID,
        "releasedByUserID": createdByUserID,
        "detainID": detainID
    }

    try {
        

        const response = await fetch(API_BASE, {
            method:  "POST",
            headers: { "Content-Type": "application/json" },
            body:    JSON.stringify(dto)
        })

        if(!response.ok)
        {
            const errText = await response.text();
            throw new Error(errText || response.statusText);
        }

        const result = await response.json();

        document.querySelector(".ReleasedApplicationID").textContent = result.releasedApplicationID;
        showToast("License release successfully!", "success");

        document.getElementById("btnRelease").disabled = true;
        document.querySelector(".ShowLicenseInfo").classList.remove("disabled");

        document.getElementById("btn-search").disabled = true;
        document.querySelectorAll("input").forEach(input => input.disabled = true);

    } catch (err) {
        console.error("Issue failed:", err);
        showToast(`Failed to get detain license info : ${err.message}`, "error");
    }


}
async function LoadLicenseFromURL()
{
    const LicenseIDFromURL = getLicenseIDFromURL();

    if(LicenseIDFromURL)
    {
        licenseIdInput.value = LicenseIDFromURL;
        LicenseID = LicenseIDFromURL;

        await searchLicense();
        await GetDetainInfo();

        document.getElementById("btn-search").disabled = true;
        document.querySelectorAll("input").forEach(input => input.disabled = true);
        document.getElementById("btnRelease").disabled = false;
        document.querySelector(".DetainLicenseID").textContent = LicenseID;
    }
}

const showLicenseHistory = document.getElementById("showLicenseHistory");
showLicenseHistory.addEventListener("click",async () => {
    window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
});
const showLicenseInfo = document.getElementById("showLicenseInfo");
showLicenseInfo.addEventListener("click", () => {
    window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${LicenseID}`;
});
const btnClose = document.getElementById("btnClose");
btnClose.addEventListener("click", () => {
    history.back(); 
});

