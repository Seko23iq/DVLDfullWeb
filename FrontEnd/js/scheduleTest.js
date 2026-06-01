import {showToast} from "./Global.js";
import {getTestTypeFeesByTestTypeID} from "./ManageTestTypesFees.js";

function toggleDropdown(id) {
    document.getElementById(id).classList.toggle('open');
}

function getIDFromURL() {
    return new URLSearchParams(window.location.search).get("LocalApplicationID");
}
function getTestTypeIDFromURL() {
    return Number(new URLSearchParams(window.location.search).get("TestTypeID"));
}

const LocalApplicationID = getIDFromURL();
const TestTypeID = getTestTypeIDFromURL();

async function getApplicantInformation() {
    try {
        const response = await fetch(`https://localhost:7223/api/TestAppointment/ApplicationInfo?LocalDrivingLicenseApplicationID=${LocalApplicationID}`);

        if (!response.ok) throw new Error("Error in fetch info!");

        const info = await response.json();

        renderApplicantInformation(info);
    }
    catch (error) {
        console.error("Error : " + error);
    }
}

async function renderApplicantInformation(info) {
    document.getElementById("dlAppID").textContent = info.localDrivingLicenseApplicationID ?? "—";
    document.getElementById("dClass").textContent  = info.applicantClass ?? "—";
    document.getElementById("appName").textContent = info.applicantName  ?? "—";

    const TestTypeFees = await getTestTypeFeesByTestTypeID(TestTypeID);
    document.getElementById("appFees").textContent = TestTypeFees != null ? `$${TestTypeFees}` : "—";

    ["dlAppID", "dClass", "appName", "appFees"].forEach(fieldId => {
        const el = document.getElementById(fieldId);
        if (el.textContent !== "—") el.classList.remove("empty");
    });
}

function buildSchedulePayload() {
    const date = document.getElementById("testDate").value;
    const userID = Number(localStorage.getItem("UserID"));

    if (!date) {
        showToast("Please select a test date.", "error");
        return null;
    }

    return {
        testTypeID: Number(TestTypeID),                                
        localDrivingLicenseApplicationID: Number(LocalApplicationID),
        appointmentDate: new Date(date).toISOString().slice(0, 10),
        createdByUserID: userID,
        isLocked: false
    };

}

async function scheduleTestSubmit() {
    const payload = buildSchedulePayload();
    if (!payload) return;

    try {

        const response = await fetch(`https://localhost:7223/api/TestAppointment/AddNewTestAppointment`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });

        if (!response.ok) 
            {
            const errorText = await response.text(); 
            console.error("Server Error:", response.status, errorText);
            }

        showToast("Test Appointment added sucessfully", "success");

        setTimeout(() => {
            window.location.href = `/Sections/Tests/Tests.html?LocalApplicationID=${LocalApplicationID}&TestTypeID=${TestTypeID}`
        }, 1000);
    }
    catch (error) {
        console.error("Error : " + error);
        alert("Something went wrong. Please try again.");
    }
}

window.addEventListener("DOMContentLoaded", () => {

    getApplicantInformation();

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    document.getElementById("testDate").min = tomorrow.toISOString().split("T")[0];

    document.querySelector(".btn-primary").addEventListener("click", () => {
        scheduleTestSubmit();
    });

    document.querySelector(".btn-ghost").addEventListener("click", () => {
        history.back();
    });
});