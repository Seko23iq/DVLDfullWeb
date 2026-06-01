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
function gettestAppointmentIDFromURL() {
    return new URLSearchParams(window.location.search).get("testAppointmentID");
}
const id = getIDFromURL();
const TestTypeID = getTestTypeIDFromURL();
const testAppointmentID = gettestAppointmentIDFromURL();

async function getApplicantInformation() {
    try {
        const response = await fetch(`https://localhost:7223/api/TestAppointment/ApplicationInfo?LocalDrivingLicenseApplicationID=${id}`);

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

    if (!date) {
        alert("Please select a test date.");
        return null;
    }

    return {
        testAppointmentID: testAppointmentID,                                   
        appointmentUpdateDate: new Date(date).toISOString().slice(0, 10),
    };

}

async function scheduleTestSubmit() {
    const payload = buildSchedulePayload();
    if (!payload) return;

    try {

        const response = await fetch(`https://localhost:7223/api/TestAppointment/Update`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload),
        });

        if (!response.ok) 
            {
            const errorText = await response.text(); // ✅ اقرأ رسالة الخطأ من الـ server
            console.error("Server Error:", response.status, errorText);
            }

        showToast("Data Saved Sucessfully.", "success")
        setTimeout(() => {
            window.location.href = `/Sections/Tests/Tests.html?LocalApplicationID=${id}&TestTypeID=${TestTypeID}`
        }, 1000);
    }
    catch (error) {
        console.error("Error : " + error);
        alert("Something went wrong. Please try again.");
    }
}

window.addEventListener("load", () => {

    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);

    document.getElementById("testDate").min = tomorrow.toISOString().split("T")[0];

    getApplicantInformation();

    document.querySelector(".btn-primary").addEventListener("click", () => {
        scheduleTestSubmit();
    });

    document.querySelector(".btn-ghost").addEventListener("click", () => {
        history.back();
    });
});
