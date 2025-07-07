<script setup>
import PersonItem from "@/components/PersonItem.vue";
import {usePersonsStore} from "@/store/persons.js";
import {computed, onMounted, ref} from "vue";

const personsStore = usePersonsStore()

onMounted(() => {
  personsStore.fetchAll()
})

const persons = computed(() => personsStore.items)
const loading = computed(() => personsStore.loading)
const error = computed(() => personsStore.error)

// --- Модалка ---
const showEditModal = ref(false)
const editPerson = ref(null)
const editForm = ref({ fullName: '', birthDate: '' })
const editFile = ref(null)

function openEditModal(person) {
  editPerson.value = person
  editForm.value = {
    fullName: person.name,
    birthDate: person.birthday ? person.birthday.slice(0, 10) : ''
  }
  editFile.value = null
  showEditModal.value = true
}
function closeEditModal() {
  showEditModal.value = false
  editPerson.value = null
  editForm.value = { fullName: '', birthDate: '' }
  editFile.value = null
}
async function saveEdit() {
  if (!editPerson.value) return
  await personsStore.update(
    editPerson.value.id,
    { name: editForm.value.fullName, birthday: editForm.value.birthDate },
    editFile.value
  )
  closeEditModal()
}
function onEditFileChange(e) {
  editFile.value = e.target.files[0] || null
}

// --- Ближайшие ---
const tabs = [
  { label: 'Все', value: 'all' },
  { label: 'Ближайшие', value: 'coming' }
]
const activeTab = ref('all')
const MONTHS = [
  'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
  'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
]
const filteredPersons = computed(() => {
  if (activeTab.value === 'all') return persons.value

  const now = new Date()
  const result = {}
  persons.value.forEach(p => {
    if (!p.birthday) return
    const bday = new Date(p.birthday)
    bday.setFullYear(now.getFullYear())
    // Если ДР уже прошёл в этом году — переносим на следующий
    if (bday < now) bday.setFullYear(now.getFullYear() + 1)
    const diffMonths = (bday.getFullYear() - now.getFullYear()) * 12 + (bday.getMonth() - now.getMonth())
    if (diffMonths < 0 || diffMonths > 5) return 
    const monthName = MONTHS[bday.getMonth()]
    if (!result[monthName]) result[monthName] = []
    result[monthName].push({ ...p, _bday: bday })
  })
 
  Object.values(result).forEach(arr => arr.sort((a, b) => a._bday - b._bday))
  // Сортируем
  const ordered = Object.keys(result)
    .sort((a, b) => {
      const aIdx = (MONTHS.indexOf(a) - now.getMonth() + 12) % 12
      const bIdx = (MONTHS.indexOf(b) - now.getMonth() + 12) % 12
      return aIdx - bIdx
    })
    .map(m => ({ month: m, persons: result[m] }))
  return ordered
})

function handleRemove(person) {
  if (confirm(`Удалить ${person.name}?`)) {
    personsStore.remove(person.id)
  }
}
</script>

<template>
  <div class="ConvoList__scrollbar-content">
    <div class="ConvoList__sticky-top">
      <nav class="ConvoList__topFiltersWrap">
        <div class="vkuiTabs__host" role="tablist">
          <div class="vkuiTabs__in">
            <div class="vkuiHorizontalScroll__host">
              <div class="vkuiHorizontalScroll__in">
                <div class="vkuiHorizontalScroll__inWrapper">
                  <div class="OrganiserViewHorizontal">
                    <div class="OrganiserViewHorizontal__tabs">
                      <div v-for="tab in tabs" :key="tab.value" class="OrganiserViewHorizontal__itemContainer">
                        <button
                          class="OrganiserViewHorizontal__item"
                          :class="{ 'vkuiTabsItem__selected': activeTab === tab.value }"
                          role="tab"
                          :aria-selected="activeTab === tab.value"
                          @click="activeTab = tab.value"
                        >
                          <span class="vkuiTabsItem__label">{{ tab.label }}</span>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </nav>
    </div>
    <div v-if="loading" class="d-flex justify-content-center align-items-center" style="height: 120px;">
      <div class="spinner-border" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else-if="error">Ошибка: {{ error }}</div>
    <section class="ConvoList__organiserSection">
      <template v-if="activeTab === 'all'">
        <PersonItem
          v-for="(person, index) in filteredPersons"
          :key="person.id"
          :person="person"
          class="ConvoList__item"
          @edit="openEditModal"
          @remove="handleRemove"
        />
      </template>
      <template v-else>
        <div v-for="group in filteredPersons" :key="group.month">
          <div class="month-title">{{ group.month }}</div>
          <PersonItem
            v-for="person in group.persons"
            :key="person.id"
            :person="person"
            class="ConvoList__item"
            @edit="openEditModal"
            @remove="handleRemove"
          />
        </div>
      </template>
    </section>
  </div>

  <!-- редакт -->
  <div v-if="showEditModal" class="modal-backdrop" @click.self="closeEditModal">
    <div class="modal-dialog modal-dialog-centered">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Редактировать</h5>
          <button type="button" class="btn-close" @click="closeEditModal" aria-label="Закрыть">×</button>
        </div>
        <div class="modal-body">
          <div class="mb-3">
            <label class="form-label">ФИО</label>
            <input v-model="editForm.fullName" class="form-control" placeholder="ФИО">
          </div>
          <div class="mb-3">
            <label class="form-label">Дата рождения</label>
            <input type="date" class="form-control" v-model="editForm.birthDate">
          </div>
          <div class="mb-3">
            <label class="form-label">Фотография (необязательно)</label>
            <input type="file" class="form-control" @change="onEditFileChange">
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-secondary" @click="closeEditModal">Отмена</button>
          <button type="button" class="btn btn-primary" @click="saveEdit">Сохранить</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ConvoList__scrollbar-content {
  width: 100%;
  min-height: 100vh;
  background: #f7f8fa;
  padding: 0;
  overflow-y: auto;
}
.ConvoList__sticky-top {
  position: sticky;
  top: 0;
  z-index: 10;
  background: #f7f8fa;
  padding-top: 8px;
}
.ConvoList__topFiltersWrap {
  margin-bottom: 8px;
}
.vkuiTabs__host {
  width: 100%;
  overflow-x: auto;
}
.OrganiserViewHorizontal__tabs {
  display: flex;
  gap: 8px;
}
.OrganiserViewHorizontal__itemContainer {
  display: flex;
}
.OrganiserViewHorizontal__item {
  background: none;
  border: none;
  padding: 8px 20px;
  border-radius: 20px;
  font-size: 1rem;
  cursor: pointer;
  color: #222;
  transition: background 0.15s, color 0.15s;
}
.vkuiTabsItem__selected {
  background: #fff;
  color: #1976d2;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  font-weight: 600;
}
.ConvoList__organiserSection {
  padding: 0 0 24px 0;
}
.ConvoList__item {
  margin: 0 12px 12px 12px;
  border-radius: 14px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.06);
  background: #fff;
  transition: box-shadow 0.15s;
}
.ConvoList__item:hover {
  box-shadow: 0 4px 16px rgba(0,0,0,0.10);
}
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.25);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}
.modal-dialog {
  max-width: 400px;
  width: 100%;
}
.modal-content {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 24px rgba(0,0,0,0.18);
  padding: 0;
  overflow: hidden;
}
.modal-header, .modal-footer {
  padding: 16px 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.modal-title {
  margin: 0;
  font-size: 1.2rem;
}
.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  line-height: 1;
}
.modal-body {
  padding: 16px 20px;
}
.month-title {
  font-size: 1.1rem;
  font-weight: 600;
  color: #1976d2;
  margin: 24px 0 8px 16px;
  letter-spacing: 0.5px;
}
</style>