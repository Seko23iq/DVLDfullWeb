
import {formatDate, showToast} from "./Global.js";


export async function searchLicense() {
    const licenseIdInput = document.getElementById("licenseIdInput");
    const licenseId = licenseIdInput.value.trim();

    if (!licenseId) {
        showToast("Please enter a License ID.", "error");
        return;
    }

    // document.querySelector(".OldLicenseID").textContent = licenseId;

    try {
        const data = await fetchLicenseInfo(licenseId);
        renderLicenseInfo(data);
        // await isLicenseExpired(licenseId, data.expirationDate);
        if(document.querySelector(".OldLicenseID"))
        {
            document.querySelector(".OldLicenseID").textContent = licenseId;
        }
        return true;

    } catch (err) {
        console.error("Failed to load license info:", err);
        showToast(`Failed to load license info, make sure there is license with this id`, "error");
        return;
    }

    document.querySelector(".LicenseID").textContent = licenseId;
}

const API_BASE = "https://localhost:7223/api/LocalDrivingApplication/license-info";


const GENDER_MAP = {
    0: "Male",
    1: "Female",
};

const ISSUE_REASON_MAP = {
    1: "New License",
    2: "Renewal",
    3: "Replacement for Damaged License",
    4: "Replacement for Lost License",
};

async function fetchLicenseInfo(LicenseID) {
    const url = `${API_BASE}?LicenseID=${LicenseID}`;

    const response = await fetch(url);

    if (!response.ok) {
        throw new Error(
        `Request failed: ${response.status} ${response.statusText}`
        );
    }

    return await response.json();
}

let isActive = true;
let isDetained = true;
let DriverID = 0;

function renderLicenseInfo(data) {

    isActive = data.isActive;
    isDetained = data.isDetained;
    DriverID = data.driverID;

    if(document.querySelector(".btn-issue"))
    {
        document.querySelector(".btn-issue").disabled  = false;
    }

    if(document.querySelector(".ShowLicenseHistory"))
    {
        document.querySelector(".ShowLicenseHistory").classList.remove("disabled");
    }

    if(document.getElementById("LocalLicenseID"))
    {
        document.getElementById("LocalLicenseID").textContent = data.licenseID;
    }
    // Class banner
    document.querySelector(".licenseClass .Class").textContent =
        data.className ?? "—";

    // Name
    document.querySelector(".Name").textContent = data.fullName ?? "—";

    // License ID
    document.querySelector(".LicenseID").textContent =
        data.licenseID != null ? `${data.licenseID}` : "—";

    // isActive badge
    document.querySelector(".IsActive").innerHTML = buildStatusBadge(
        data.isActive
    );

    // National No
    document.querySelector(".NationalNo").textContent = data.nationalNo ?? "—";

    // Date Of Birth
    document.querySelector(".DateOfBirth").textContent = formatDate(
        data.dateOfBirth
    );

    // Gender
    document.querySelector(".Gendor").textContent =
        GENDER_MAP[data.gendor] ?? "—";

    // Driver ID
    document.querySelector(".DriverID").textContent =
        data.driverID != null ? `${String(data.driverID)}` : "—";

    // Issue Date
    document.querySelector(".IssueDate").textContent = formatDate(data.issueDate);

    // Expiration Date
    document.querySelector(".ExpirationDate").textContent = formatDate(
        data.expirationDate
    );

    // Issue Reason
    document.querySelector(".IssueReason").textContent =
        ISSUE_REASON_MAP[data.issueReason] ?? "—";

    // Is Detained — the JSON doesn't include isDetained, default to false
    document.querySelector(".IsDetained").innerHTML = buildDetainedBadge(
        data.isDetained ?? false
    );

    // Notes
    document.querySelector(".Notes").textContent =
        data.notes?.trim() || "No additional notes for this license.";
}
function buildStatusBadge(isActive) {
    if (isActive) {
        return `<span class="status-badge status-active">Active</span>`;
    }

    return `<span class="status-badge status-inactive">Inactive</span>`;
}

function buildDetainedBadge(isDetained) {
    if (isDetained) {
        return `<span class="status-badge status-detained">Detained</span>`;
    }
    return `<span class="status-badge status-free">Not Detained</span>`;
}



async function isLicenseExpired(LicenseID, expirationDate) {
    try {
        const response = await fetch(`https://localhost:7223/api/RenewLicense/IsLicenseExpire/${LicenseID}`);
        if (!response.ok) {
            showToast("Error fetching expiry data", "error");
            return;
        }

        const isExpired = await response.json();

        if (!isExpired) {
            showToast(
                `Selected License is not yet expired, it will expire on: ${formatDate(expirationDate)}`,
                "error"
            );
            document.querySelector(".btn-issue").disabled = true;
            document.querySelector(".ShowLicenseInfo").classList.add("disabled");
        } else {
            document.querySelector(".btn-issue").disabled = false;
        }
    } catch (error) {  // ✅ إصلاح: كان error غير معرّف في catch
        console.error("Error: " + error);
    }
}