<script setup>
import {ref} from "vue";
import {usePersonsStore} from "@/store/persons.js";

const personsStore = usePersonsStore()

const person = ref({
  fullName: '',
  birthDate: ''
})
const file = ref(null)

const onFileChange = (e) => {
  file.value = e.target.files[0] || null;
  console.log('file:', file.value);
}

const push_person = async () => {
  if (!file.value) {
    alert('Выбор фото обязательный!');
    return;
  }
  await personsStore.create({
    name: person.value.fullName,
    birthday: person.value.birthDate
  }, file.value)
  person.value = { fullName: '', birthDate: '' }
  file.value = null
}
</script>

<template>
  <main class="p-5">
    <h1>Добавить именинника</h1>
    <div>
      <label class="form-label">ФИО</label>
      <input v-model="person.fullName" class="form-control" placeholder="ФИО">
    </div>
    <div>
      <label class="form-label">Дата рождения</label>
      <input type="date" class="form-control" v-model="person.birthDate">
    </div>
    <div>
      <label class="form-label">Фотография</label>
      <input type="file" class="form-control" @change="onFileChange">
    </div>
    <button @click="push_person" class="btn btn-primary mt-3">Добавить</button>
  </main>
</template>

<style scoped>
</style>
