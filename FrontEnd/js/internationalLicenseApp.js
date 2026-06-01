
import { searchLicense } from "./searchLicense.js";
import { showToast,formatDate } from "./Global.js";
import { getApplicationTypeFeesByApplicationID } from "./ManageApplicationTypeFees.js";

const API_BASE = "https://localhost:7223/api/LocalDrivingApplication/license-info";


/* ── Helpers ───────────────────────────────────────────────────── */
function getUrlParam(name) {
  const params = new URLSearchParams(window.location.search);
  return params.get(name);
}

document.addEventListener("DOMContentLoaded",async () => {
    document.querySelector(".internationIssueDate").textContent = formatDate(Date.now());
    document.querySelector(".ApplicationDate").textContent = formatDate(Date.now());

    document.querySelector(".internationalExpirationDate").textContent = formatDate(new Date(new Date().setFullYear(new Date().getFullYear() + 1)));

    const userName = localStorage.getItem("UserName");
    document.querySelector(".createdBy").textContent = userName;

    const InternationalApplicationFees = await getApplicationTypeFeesByApplicationID(6);
    document.querySelector(".fees-value").textContent = InternationalApplicationFees + '$';


    document.querySelector(".btn-issue").disabled  = true;
    document.querySelector(".ShowLicenseInfo").classList.add("disabled");
    document.querySelector(".ShowLicenseHistory").classList.add("disabled");
});

const LocalLicenseID = document.getElementById("LocalLicenseID");

let applicationID = 0;

/* ── Issue Application ─────────────────────────────────────────── */
const API_ISSUE = "https://localhost:7223/api/InternationalApplication/AddNewInternationalLicense";

async function issueApplication() {
  const btn = document.querySelector(".btn-issue");

  const isActive = document.querySelector(".status-active").textContent;
  const ClassName = document.querySelector(".Class").textContent;


  if(isActive !== "Active")
  {
    showToast("Your license is not active you can't make internation license active it first.", "error");
    document.querySelector(".btn-issue").disabled  = true;
    return;
  }
  if(ClassName !== "Class 3 - Ordinary driving license")
  {
    showToast("Selected License should be Class 3 - Ordinary driving license, select another one.", "error");
    document.querySelector(".btn-issue").disabled  = true;
    return;
  }

  const hasInternationalLicense = await HasInternationalLicense();
  
  console.log(hasInternationalLicense);
  if(hasInternationalLicense)
    {
      showToast(`Person already have an international license, you can't issue more than one international license for the same person.`, "error");
      document.querySelector(".btn-issue").disabled  = true;
      return;
    }


  // ── Gather required data ──────────────────────────────────────
  const driverIDText  = document.querySelector(".DriverID")?.textContent?.trim();
  const licenseIDText = document.querySelector(".LicenseID")?.textContent?.trim();
  const localLicenseID = document.getElementById("LocalLicenseID")?.textContent?.trim();
  const userID        = Number(localStorage.getItem("UserID"));
  const applicantPersonID = await GetPersonIDByLicenseID();
  const paidFees = await getApplicationTypeFeesByApplicationID(6);

  // Validate — all fields must be present and not placeholders
  if (
    !driverIDText  || driverIDText  === "—" || driverIDText  === "-" ||
    !licenseIDText || licenseIDText === "—" || licenseIDText === "-" ||
    !localLicenseID || localLicenseID === "—" || localLicenseID === "-" ||
    !userID
  ) {
    showToast("Please search for a valid license before issuing.", "error");
    return;
  }

  const driverID             = parseInt(driverIDText,   10);
  const applicationID        = parseInt(licenseIDText,  10);
  const issuedUsingLocalLicenseID = parseInt(localLicenseID, 10);
  const createdByUserID      = parseInt(userID, 10);

  // ── Build dates ───────────────────────────────────────────────
  const issueDate       = new Date().toISOString();
  const expirationDate  = new Date(
    new Date().setFullYear(new Date().getFullYear() + 1)
  ).toISOString();

  // // ── Build DTO ─────────────────────────────────────────────────
  const dto = {
    applicantPersonID,
    driverID,
    issuedUsingLocalLicenseID,
    issueDate,
    expirationDate,
    createdByUserID,
    paidFees,
  };

  // // ── UI: loading state ─────────────────────────────────────────
  btn.disabled    = true;
  btn.innerHTML   = `
    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
      fill="none" stroke="currentColor" stroke-width="2.5"
      stroke-linecap="round" stroke-linejoin="round" class="spin-icon">
      <path d="M21 12a9 9 0 1 1-6.219-8.56"/>
    </svg>
    Issuing…`;

    
  try {
    const response = await fetch(API_ISSUE, {
      method:  "POST",
      headers: { "Content-Type": "application/json" },
      body:    JSON.stringify(dto),
    });


    if (!response.ok) {
      const errText = await response.text();
      throw new Error(`${response.status} — ${errText || response.statusText}`);
    }

    const result = await response.json();

    // ── Success ───────────────────────────────────────────────
    showToast(
      result.message ?? "International license issued successfully.",
      "success"
    );

    // Update the I.L. Application ID field with the returned ID
    if (result.result.applicationID != null) {
      const ilAppID = document.querySelector(".appInfo .appField:first-child .appFieldValue");
      if (ilAppID) ilAppID.textContent = `${String(result.result.applicationID)}`;
    }
    if (result.result.internationalLicenseID != null) {
      const ilAppID = document.querySelector(".appInfo .appField:nth-child(2) .appFieldValue");
      if (ilAppID) ilAppID.textContent = `${String(result.result.internationalLicenseID)}`;
    }

    document.querySelector(".ShowLicenseInfo").classList.remove("disabled");
    document.querySelector(".filter-input").disabled  = true;
    document.querySelector(".btn-search").disabled  = true;

    // Keep button disabled — already issued
    btn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
        fill="none" stroke="currentColor" stroke-width="2"
        stroke-linecap="round" stroke-linejoin="round">
        <polyline points="20 6 9 17 4 12"/>
      </svg>
      Issued`;

  } catch (err) {
    console.error("issueApplication failed:", err);
    showToast(`Failed to issue license. ${err.message}`, "error");

    // Restore button so user can retry
    btn.disabled  = false;
    btn.innerHTML = `
      <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24"
        fill="none" stroke="currentColor" stroke-width="2"
        stroke-linecap="round" stroke-linejoin="round">
        <polyline points="20 6 9 17 4 12"/>
      </svg>
      Issue`;
  }
}

async function HasInternationalLicense()
{
  const DriverID = document.querySelector(".DriverID").textContent;

  const Api = `https://localhost:7223/api/InternationalApplication/HasInternationalLicense?DriverID=${Number(DriverID)}`;
  try
  {
    const response = await fetch(Api);
    const data = await response.json();
    const hasInternationalLicense = data.hasInternationalLicense;

    console.log(hasInternationalLicense);
    return hasInternationalLicense;
    
  }
  catch(error)
  {
    console.error("Error : " + error);
  }
}

async function hasActiveInternationalLicenseID()
{
  const Api = `https://localhost:7223/api/InternationalApplication/active-license/${DriverID}`;
  try
  {
    const response = await fetch(Api);
    const data = await response.json();
    const LicenseID = data.activeInternationalLicenseID;


    if(LicenseID > 0)
    {
      showToast(`Person already have an active international license with ID = ${LicenseID}`, "error");
      document.querySelector(".btn-issue").disabled  = true;
    }
  }
  catch(error)
  {
    console.error("Error : " + error);
  }
}

async function GetPersonIDByLicenseID()
{

  const Api = `https://localhost:7223/api/InternationalApplication/GetPersonIDByLicenseID?LicenseID=${LocalLicenseID.textContent}`;
  try
  {
    const response = await fetch(Api);
    const data = await response.json();
    const PersonID = data.personID;
    return PersonID;
  }
  catch(error)
  {
    console.error("Error : " + error);
  }
}

const btnCancel = document.getElementById("btnCancel");
btnCancel.addEventListener("click", () => {
  history.back();
})

const btnIssue = document.getElementById("btnIssue");
btnIssue.addEventListener("click",  () => {
  issueApplication();
})

let PersonID;
const btnSearch = document.getElementById("btnSearch");
btnSearch.addEventListener("click", async () => {
  await searchLicense();

  const LicenseID = document.getElementById("licenseIdInput").value;

  PersonID = await GetPersonIDByLicenseID(LicenseID);

})

const ShowLicenseInfo = document.getElementById("ShowLicenseInfo");
ShowLicenseInfo.addEventListener("click", () => {
  const InternationID = document.querySelector(".InternationalID").textContent;
  window.location.href = `/Sections/Applications/InternationalApplication/InternationalApplicationInfo.html?InternationID=${InternationID}`;
})
const btnShowLicenseHistory = document.getElementById("btnShowLicenseHistory");
btnShowLicenseHistory.addEventListener("click", () => {
  window.location.href = `/Sections/Applications/LocalApplication/LicenseHistory.html?id=${PersonID}`;
})