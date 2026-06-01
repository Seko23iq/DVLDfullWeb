export async function fetchLicenseClassInfo(ClassName) {
    try {
        const response = await fetch(`https://localhost:7223/api/LicenseClass/LicenseClassInfo/${ClassName}`);
        if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
        const data = await response.json();
        return data;
    }
    catch (err) {
        console.error("Failed to fetch license class info:", err);
        throw err;
    }
}