let allApplicationTypes    = [];   

export async function getApplicationTypeFeesByApplicationID(ApplicationTypeID)
{
    try {
        let url = "https://localhost:7223/api/ApplicationTypes/All";

        const response = await fetch(url);

        if (!response.ok) throw new Error("Failed to fetch");

        allApplicationTypes = await response.json();

        const fees = allApplicationTypes.find(s => s.id === ApplicationTypeID )?.fees;

        return fees;
    } catch (err) {
        console.error(err);
    }
}