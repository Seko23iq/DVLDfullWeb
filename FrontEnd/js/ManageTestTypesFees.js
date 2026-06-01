let allApplicationTestTypes    = [];   

export async function getTestTypeFeesByTestTypeID(TestTypeID)
{
    try {
        let url = "https://localhost:7223/api/ApplicationTestTypes/All";

        const response = await fetch(url);

        if (!response.ok) throw new Error("Failed to fetch");

        allApplicationTestTypes = await response.json();

        const fees = allApplicationTestTypes.find(s => s.id === TestTypeID )?.fees;

        return fees;
    } catch (err) {
        console.error(err);
    }
}