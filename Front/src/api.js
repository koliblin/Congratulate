// api.js — модуль для работы с сервером ASP.NET Core Web API
const API_URL = 'http://localhost:5087/api/Person'; 

export async function getAllPersons() {
    const res = await fetch(API_URL);
    return await res.json();
}

export async function getComingPersons() {
    const res = await fetch(`${API_URL}/upcoming`);
    return await res.json();
}

export async function getPerson(id) {
    const res = await fetch(`${API_URL}/${id}`);
    return await res.json();
}

export async function createPerson(person, file) {
    const formData = new FormData();
    formData.append('name', person.name);
    formData.append('birthday', person.birthday);
    if (file) {
        formData.append('photo', file);
    }
    console.log('photo in formData:', formData.get('photo'));// проверка связи
    const res = await fetch(API_URL, {
        method: 'POST',
        body: formData
    });
    return await res.json();
}

export async function updatePerson(id, person, file) {
    const formData = new FormData();
    formData.append('name', person.name);
    formData.append('birthday', person.birthday);
    if (file) {
        formData.append('photo', file);
    }
    await fetch(`${API_URL}/${id}`, {
        method: 'PUT',
        body: formData
    });
}

export async function deletePerson(id) {
    await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
}

export async function uploadPhoto(id, file) {
    const formData = new FormData();
    formData.append('file', file);
    const res = await fetch(`${API_URL}/${id}/photo`, {
        method: 'POST',
        body: formData
    });
    return await res.json();
}

export function getPhotoUrl(person) {
    if (!person.photoPath) return null;
    return `http://localhost:5087/api/person/photo/${person.photoPath}`;
} 