import {showToast} from "./Global.js";
function getLocalApplicationIDFromURL() {
    const params = new URLSearchParams(window.location.search);
    return params.get("id");
}
const LocalApplicationID = getLocalApplicationIDFromURL();

const appIdDisplay = document.getElementById("appIdDisplay");
const btnScheduleVision = document.getElementById("btn-schedule-vision");
const btnScheduleWriting = document.getElementById("btn-schedule-writing");
const btnScheduleStreet = document.getElementById("btn-schedule-street");
const btnShowLicense = document.getElementById("btn-showLicense");
const btnIssueLicense = document.getElementById("btn-issueLicense");

let LicenseID = 0;
let PersonID = 0;
let passedTest = 0;
// let isActive;
window.addEventListener("load", async () =>
{
    appIdDisplay.textContent = LocalApplicationID;
    GetPassedTestForThisLocalApplication();
    LicenseID = await GetLicenseIDFromApplicationID();
    PersonID = await GetPersonIDFromApplicationID();
    passedTest = await GetPassedTestForThisLocalApplication();
    updateScheduleButtons(passedTest, LicenseID);


    // isActive = localStorage.getItem("isActive");
})
btnIssueLicense.addEventListener("click", () => {
    // if(isActive === "false")
    // {
    //     showToast("You are not active user in system, you cannot issue a license.", "error");
    //     return;
    // }

    window.location.href = `/Sections/Applications/LocalApplication/IssueForFirstTime.html?LocalApplicationID=${LocalApplicationID}`;
})
btnShowLicense.addEventListener("click", () => {
    if(LicenseID !== -1)
    {
        window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${LicenseID}`;
    }
    else 
    {
        showToast("There is no license.", "error");
    }
})
function updateScheduleButtons(passedTest, licenseID) {
    // تعطيل الكل أولاً
    btnScheduleVision.disabled  = true;
    btnScheduleWriting.disabled = true;
    btnScheduleStreet.disabled  = true;
    btnShowLicense.disabled  = true;
    btnIssueLicense.disabled  = true;

    if (passedTest === 0) {
        btnScheduleVision.disabled = false;

    } else if (passedTest === 1) {
        btnScheduleWriting.disabled = false;

    } else if (passedTest === 2) {
        btnScheduleStreet.disabled = false;
    }
    
    if(passedTest === 3 && licenseID <= 0)
    {
        btnIssueLicense.disabled = false;
    }
    else 
    {
        btnIssueLicense.disabled = true;
    }
    if(passedTest === 3 && licenseID >= 0)
    {
        btnShowLicense.disabled = false;
    }
    else 
    {
            btnShowLicense.disabled = true;
    }
    
    // passedTest === 3 — الكل معطل (اكتمل)
}
async function GetPassedTestForThisLocalApplication()
{
    try {
        const response = await fetch(`https://localhost:7223/api/LocalDrivingApplication/PassedTest?LocalDrivingLicenseApplicationID=${LocalApplicationID}`);

        if(!response.ok)
        {
            throw new Error("Error in geting PassedTest");
        } 
        
        const PassedTest = await response.json();
        
        // updateScheduleButtons(PassedTest);

        return PassedTest;
    } catch (error) {
        console.error("Error : " + error );
    }
}

const showPersonHistory = document.getElementById("showPersonHistory");
showPersonHistory.addEventListener("click", ShowPersonHistory);

function ShowPersonHistory() {
    window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
}














































const showApplicationDetails = document.getElementById("showApplicationDetails");
showApplicationDetails.addEventListener("click", () => {
    window.location.href = `/Sections/Applications/LocalApplication/localApplicationInfo.html?LocalApplicationID=${LocalApplicationID}`;
});

async function GetLicenseIDFromApplicationID()
{
    try {
        const ApplicationID = await GetApplicationID(LocalApplicationID);

        const response = await fetch(`https://localhost:7223/api/LocalDrivingApplication/GetLicenseIDforApplicationID?ApplicationID=${ApplicationID}`);

        if(!response.ok)
        {
            throw new Error("Error in geting LicenseID");
        }
        const LicenseID = await response.json();
        return LicenseID.licenseID ;
    } catch (error) {
        console.error("Error : " + error );
    }
}

async function GetPersonIDFromApplicationID()
{
    try {

        const response = await fetch(`https://localhost:7223/api/LocalDrivingApplication/GetPersonIDforLocalApplicationID?LocalApplicationID=${LocalApplicationID}`);

        if(!response.ok)
        {
            throw new Error("Error in geting PersonID");
        }
        const PersonID = await response.json();

        return PersonID.personID;
    } catch (error) {
        console.error("Error : " + error );
    }
}
// ── Fetch Application Info ────────────────────
async function GetApplicationID(id) {
    try {
        const res = await fetch(`https://localhost:7223/api/LocalDrivingApplication/ApplicationBasicInfo?LocalDrivingLicenseApplicationID=${id}`);
        if (!res.ok) throw new Error("Failed to fetch application info");
        const data = await res.json();
        return data.applicationID;
    } catch (err) {
        console.error(err);
    }
}
window.scheduleTest = function(type) {
    let TestTypeID = 0;

    if(type === "Vision")
    {
        TestTypeID = 1;
    }
    else if(type === "Writing")
    {
        TestTypeID = 2;
    }
    else if(type === "Street")
    {
        TestTypeID = 3;
    }

    window.location.href = `/Sections/Tests/Tests.html?LocalApplicationID=${LocalApplicationID}&TestTypeID=${TestTypeID}`;
}
function toggleDropdown(id) {
    document.getElementById(id).classList.toggle('open');
}