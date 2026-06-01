import {formatDate, showError, showToast,GetPersonIDbyLicenseID} from "./Global.js";
import {searchLicense} from "./searchLicense.js";

const btnSerach = document.getElementById("btn-search");
const licenseIdInput = document.getElementById("licenseIdInput");
const btnDetain = document.getElementById("btnDetain");

document.addEventListener("DOMContentLoaded", () => {

    document.querySelector(".DetainDate").textContent = formatDate(Date.now());

    const userName = localStorage.getItem("UserName");
    document.querySelector(".CreatedBy").textContent = userName;

    document.getElementById("btnDetain").disabled = true;
    document.querySelector(".ShowLicenseInfo").classList.add("disabled");
    document.querySelector(".ShowLicenseHistory").classList.add("disabled");

})

let PersonID = 0;
let LicenseID = 0;
btnSerach.addEventListener("click", async () => {
    await searchLicense();

    LicenseID = document.querySelector(".LicenseID").textContent;
    if(LicenseID !== "-")
    {
        PersonID = await GetPersonIDbyLicenseID(LicenseID);
    }

    document.querySelector(".DetainLicenseID").textContent = licenseIdInput.value;
    document.getElementById("btnDetain").disabled = false;

    isLicenseActive();
    isLicenseDetain();
})

function isLicenseActive()
{
    const IsActive = document.querySelector(".IsActive .status-badge").textContent;

    if(IsActive === "Inactive")
    {
        showToast("Your license is not active you cannot do this process", "error");
        btnDetain.disabled = true;
        return;
    }
}
function isLicenseDetain()
{
    const IsDetained = document.querySelector(".IsDetained .status-badge").textContent;

    if(IsDetained === "Detained")
    {
        showToast("Your license is already detained!", "error");
        btnDetain.disabled = true;
        return;
    }
}




btnDetain.addEventListener("click", () => {
    DetainLicense();
})


async function DetainLicense()
{

    const API_BASE = "https://localhost:7223/api/DetainReleaseLicense/DetainLicense";

    const LicenseID = document.querySelector(".DetainLicenseID").textContent;
    const detainDate = new Date();
    const fineFees = document.querySelector(".fine-fees-input").value;
    const createdByUserID = localStorage.getItem("UserID");

    if(!fineFees)
    {
        showToast("Enter a fine Fees.", "error");
        return;
    }
    const dto = {
        licenseID : LicenseID,
        detainDate : detainDate,
        fineFees : fineFees,
        createdByUserID : createdByUserID
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
        console.log(result);
        showToast("License detained successfully!", "success");
        
        document.querySelector(".DetainID").textContent = result.detainID;

        document.getElementById("btnDetain").disabled = true;
        document.getElementById("btn-search").disabled = true;
        document.querySelector(".ShowLicenseInfo").classList.remove("disabled");
        document.querySelectorAll("input").forEach(input => input.disabled = true);


    } catch (err) {
        console.error("Issue failed:", err);
        showToast(`Failed to detain license: ${err.message}`, "error");
    }


}


const ShowLicenseHistory = document.getElementById("showLicenseHistory");
ShowLicenseHistory.addEventListener("click", () => {
    if(PersonID)
    {
        window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
    }
})

const showLicenseInfo = document.getElementById("showLicenseInfo");
showLicenseInfo.addEventListener("click", () => {
    if(LicenseID)
    {
        window.location.href = `/Sections/Applications/LocalApplication/DriverLicenseInfo.html?LicenseID=${LicenseID}`;
    }
})
const btnClose = document.getElementById("btnClose");
btnClose.addEventListener("click", () => {
    history.back();
})


