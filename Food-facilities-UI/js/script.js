// Search type selection
const searchTypeSelect = document.getElementById('searchTypeSelect');
const nameSearchForm = document.getElementById('nameSearchForm');
const streetSearchForm = document.getElementById('streetSearchForm');
const coordinatesSearchForm = document.getElementById('coordinatesSearchForm');

searchTypeSelect.addEventListener('change', function () {
    // Hide all forms
    nameSearchForm.classList.remove('active');
    streetSearchForm.classList.remove('active');
    coordinatesSearchForm.classList.remove('active');

    // Show selected form
    switch (this.value) {
        case 'name':
            nameSearchForm.classList.add('active');
            break;
        case 'street':
            streetSearchForm.classList.add('active');
            break;
        case 'coordinates':
            coordinatesSearchForm.classList.add('active');
            break;
    }

    // Clear results table
    const resultsTable = document.getElementById('resultsTable');
    const resultsBody = document.getElementById('resultsBody');
    resultsBody.innerHTML = '';
    resultsTable.style.display = 'none';
});

// Street search validation
const streetInput = document.getElementById('streetInput');
const searchByStreetBtn = document.getElementById('searchByStreetBtn');

streetInput.addEventListener('input', function () {
    searchByStreetBtn.disabled = this.value.length < 3;
});

// Search by name
document.getElementById('searchByNameBtn').addEventListener('click', async function () {
    const facilityName = document.getElementById('facilityNameInput').value;
    const status = document.getElementById('statusSelect').value;

    if (!facilityName) {
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">Please enter a facility name</td></tr>';
        resultsTable.style.display = 'table';
        return;
    }

    showLoading();
    try {
        // Build the URL with optional status parameter
        let url = `http://localhost:8080/api/FoodFacilities/searchbyapplicantname?applicantName=${facilityName}`;
        if (status) {
            url += `&status=${status}`;
        }

        const response = await fetch(url);
        if (!response.ok) {
            if (response.status === 400) {
                const errorText = await response.text();
                const resultsTable = document.getElementById('resultsTable');
                const resultsBody = document.getElementById('resultsBody');
                resultsBody.innerHTML = `<tr><td colspan="5" class="text-center">${errorText}</td></tr>`;
                resultsTable.style.display = 'table';
                return;
            }
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        console.log('API Response:', data);
        displayResults(data);
    } catch (error) {
        console.error('Error:', error);
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">No Results Found</td></tr>';
        resultsTable.style.display = 'table';
    } finally {
        hideLoading();
    }
});

// Search by street
document.getElementById('searchByStreetBtn').addEventListener('click', async function () {
    const street = streetInput.value;

    if (!street) {
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">Please enter a street name</td></tr>';
        resultsTable.style.display = 'table';
        return;
    }

    showLoading();
    try {
        const response = await fetch(`http://localhost:8080/api/FoodFacilities/searchbystreetname?streetName=${street}`);
        if (!response.ok) {
            if (response.status === 400) {
                const errorText = await response.text();
                const resultsTable = document.getElementById('resultsTable');
                const resultsBody = document.getElementById('resultsBody');
                resultsBody.innerHTML = `<tr><td colspan="5" class="text-center">${errorText}</td></tr>`;
                resultsTable.style.display = 'table';
                return;
            }
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        console.log('API Response:', data);
        displayResults(data);
    } catch (error) {
        console.error('Error:', error);
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">No Results Found</td></tr>';
        resultsTable.style.display = 'table';
    } finally {
        hideLoading();
    }
});

// Search by coordinates
document.getElementById('searchByCoordsBtn').addEventListener('click', async function () {
    const latitude = document.getElementById('latitudeInput').value;
    const longitude = document.getElementById('longitudeInput').value;
    const status = document.getElementById('coordinatesStatusSelect').value;

    if (!latitude || !longitude) {
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">Please enter both latitude and longitude</td></tr>';
        resultsTable.style.display = 'table';
        return;
    }

    showLoading();
    try {
        // Build the URL with optional status parameter
        let url = `http://localhost:8080/api/FoodFacilities/searchbylocation?latitude=${latitude}&longitude=${longitude}`;
        if (status) {
            url += `&status=${status}`;
        }

        const response = await fetch(url);
        if (!response.ok) {
            if (response.status === 400) {
                const errorText = await response.text();
                const resultsTable = document.getElementById('resultsTable');
                const resultsBody = document.getElementById('resultsBody');
                resultsBody.innerHTML = `<tr><td colspan="5" class="text-center">${errorText}</td></tr>`;
                resultsTable.style.display = 'table';
                return;
            }
            throw new Error('Network response was not ok');
        }
        const data = await response.json();
        console.log('API Response:', data);
        displayResults(data);
    } catch (error) {
        console.error('Error:', error);
        const resultsTable = document.getElementById('resultsTable');
        const resultsBody = document.getElementById('resultsBody');
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">No Results Found</td></tr>';
        resultsTable.style.display = 'table';
    } finally {
        hideLoading();
    }
});

function displayResults(data) {
    const resultsTable = document.getElementById('resultsTable');
    const resultsBody = document.getElementById('resultsBody');

    resultsBody.innerHTML = '';

    if (!data || data.length === 0) {
        resultsBody.innerHTML = '<tr><td colspan="5" class="text-center">No facilities found</td></tr>';
    } else {
        data.forEach(facility => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${facility.applicant}</td>
                <td>${facility.facilityType}</td>
                <td>${facility.address}</td>
                <td>${facility.status}</td>
                <td>(${parseFloat(facility.latitude).toFixed(6)}°, ${parseFloat(facility.longitude).toFixed(6)}°)</td>
            `;
            resultsBody.appendChild(row);
        });
    }

    resultsTable.style.display = 'table';
}

function showLoading() {
    document.getElementById('loadingIndicator').style.display = 'block';
    document.getElementById('resultsTable').style.display = 'none';
}

function hideLoading() {
    document.getElementById('loadingIndicator').style.display = 'none';
} 