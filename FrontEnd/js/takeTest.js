import {showToast, formatDate} from "./Global.js";
import {getTestTypeFeesByTestTypeID} from "./ManageTestTypesFees.js";

const API_BASE = 'https://localhost:7223/api';

// ── Read TestAppointmentID from URL query string ─────────────────
const params            = new URLSearchParams(window.location.search);
const testAppointmentID = params.get('TestAppointmentID');
const TestTypeID = Number(params.get('TestTypeID'));
let  drivingLicenseApplicationID = 0;
// ── DOM refs ─────────────────────────────────────────────────────
const elAppId   = document.getElementById('dlAppId');
const elClass   = document.getElementById('dClass');
const elName    = document.getElementById('applicantName');
const elTrial   = document.getElementById('trial');
const elDate    = document.getElementById('testDate');
const elFees    = document.getElementById('fees');
const elTestId  = document.getElementById('testId');


function setLoading(isLoading) {
    const cards = document.querySelectorAll('.info-value');
    cards.forEach(el => {
        el.classList.toggle('skeleton', isLoading);
        if (isLoading) el.textContent = '—';
    });
}

// ── Fetch applicant info ─────────────────────────────────────────
async function loadTestInfo() {
    if (!testAppointmentID) {
        showError('No TestAppointmentID provided in the URL.');
        return;
    }

    const TestTypeFees = await getTestTypeFeesByTestTypeID(TestTypeID);

    setLoading(true);

    try {
        const res = await fetch(
            `${API_BASE}/TestAppointment/TakerTestInfo?TestAppointmentID=${testAppointmentID}`
        );

        if (!res.ok) {
            throw new Error(`Server returned ${res.status} ${res.statusText}`);
        }

        const data = await res.json();

        drivingLicenseApplicationID = data.drivingLicenseApplicationID;
        // Populate info cards
        elAppId.textContent  = data.drivingLicenseApplicationID ?? '—';
        elClass.textContent  = data.licenseClass                ?? '—';
        elName.textContent   = data.name                        ?? '—';
        elDate.textContent   = formatDate(data.date);
        elFees.textContent   = TestTypeFees + "$";

        elTrial.textContent  = params.get('Trial') ?? '0';
        elTestId.textContent = 'Not Taken Yet';               // ✅ FIX 3: was '-'

    } catch (err) {
        showError(err.message);
    } finally {
        setLoading(false);
    }
}

function showError(message) {
    const banner = document.getElementById('errorBanner');
    if (banner) {
        banner.textContent = `⚠ ${message}`;
        banner.hidden = false;
    } else {
        console.error(message);
    }
}

// ── Action handlers ──────────────────────────────────────────────
function handleCancel() {
    history.back();
}

async function handleSave() {
    const resultEl = document.querySelector('input[name="result"]:checked');
    const notes    = document.getElementById('notes').value.trim();
    const userID = Number(localStorage.getItem("UserID"));

    if (!resultEl) {
        alert('Please select a result (Pass or Fail) before saving.');
        return;
    }

    const testResult = resultEl.value === 'pass' ? 1 : 0;     

    const payload = {
        testAppointmentID: Number(testAppointmentID),
        testResult: testResult,                                            
        notes: notes,
        createdByUserID: userID,
    };

    const saveBtn = document.querySelector('.btn--save');
    saveBtn.disabled = true;
    saveBtn.textContent = 'Saving…';

    try {
        const res = await fetch(`${API_BASE}/TestAppointment/TakeTest`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload),
        });

        if (!res.ok) throw new Error(`Save failed: ${res.status}`);

        const data = await res.json();

        elTestId.textContent = data.newID;


        showToast(`Take test added sucessfully with test ID = ${data.newID}`, "success")

        setTimeout(() => {
            window.location.href = `/Sections/Applications/LocalApplication/ProcessingLA.html?id=${drivingLicenseApplicationID}`;
        }, 1000);

    } catch (err) {
        alert(`Error saving: ${err.message}`);
    } finally {
        saveBtn.disabled = false;
        saveBtn.innerHTML = `
            <svg viewBox="0 0 20 20" fill="none" stroke="currentColor" stroke-width="1.8">
                <path d="M4 10l4.5 4.5L16 6" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            Save`;
    }
}


const btnSave = document.getElementById('btnSave');
const btnCancel = document.getElementById('btnCancel');

btnSave.addEventListener('click', handleSave);
btnCancel.addEventListener('click', handleCancel);
// ── Init ─────────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', loadTestInfo);