export function formatDate(dateStr) {
    if (!dateStr) return '—';
    const date = new Date(dateStr);
    if (isNaN(date)) return dateStr;
    const day   = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year  = date.getFullYear();
    return `${day}/${month}/${year}`;
}

export function showError(message) {
    document.querySelectorAll(".licenseInfo div span:last-child").forEach((el) => {
        el.textContent = "—";
        el.style.color = "var(--muted)";
    });
    document.querySelector(".licenseClass .Class").textContent = "Error";

    const notes = document.querySelector(".Notes");
    if (notes) {
        notes.textContent = message;
        notes.style.color = "#a32d2d";
    }
}


export function showToast(message, type = "success") {
    const toast = document.getElementById('toast');

    toast.textContent = message;

    toast.classList.remove('success', 'error');

    if (type.toLowerCase() === "error") {
        toast.classList.add('error');
    } else {
        toast.classList.add('success');
    }

    toast.classList.add('show');

    setTimeout(() => {
        toast.classList.remove('show');
    }, 3500);
}


export async function GetPersonIDbyLicenseID(LicenseID)
{
    try
    {
        const response = await fetch("https://localhost:7223/api/InternationalApplication/GetPersonIDByLicenseID?LicenseID=" + LicenseID);
        if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
        const data = await response.json();
        return data.personID;
    }
    catch (err) {
        console.error("Failed to fetch person ID:", err);
        throw err;
    }
}