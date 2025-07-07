<template>
  <div class="person-card">
    <img v-if="photoUrl" :src="photoUrl" class="person-avatar" alt="Фото именинника">
    <div class="person-info">
      <div class="person-name">{{ person.name }}</div>
      <div class="person-date">{{ formattedDate }}</div>
    </div>
    <button class="menu-trigger-btn" @click.stop="toggleMenu" aria-label="Меню действий">
      <span class="menu-dots">⋮</span>
    </button>
    <div v-if="showMenu" class="menu-dropdown">
      <div class="menu-item" @click="edit">Редактировать</div>
      <div class="menu-item" @click="remove">Удалить</div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from 'vue'
import { getPhotoUrl } from '@/api.js'

const props = defineProps({
  person: Object,
})

const emit = defineEmits(['remove', 'edit'])

function remove(){
  emit('remove', props.person)
}
function edit(){
  emit('edit', props.person)
  showMenu.value = false
}

const photoUrl = computed(() => getPhotoUrl(props.person))
const formattedDate = computed(() => {
  if (!props.person.birthday) return ''
  const date = new Date(props.person.birthday)
  return date.toLocaleDateString('ru-RU', { year: 'numeric', month: 'long', day: 'numeric' })
})

const showMenu = ref(false)
function toggleMenu() {
  showMenu.value = !showMenu.value
}
function closeMenu(e) {
  if (!e.target.closest('.menu-dropdown') && !e.target.closest('.menu-trigger')) {
    showMenu.value = false
  }
}
onMounted(() => {
  window.addEventListener('click', closeMenu)
})
onBeforeUnmount(() => {
  window.removeEventListener('click', closeMenu)
})
</script>

<style scoped>
.person-card {
  display: flex;
  align-items: center;
  padding: 12px 16px;
  background: transparent;
  border-radius: 14px;
  position: relative;
  min-height: 64px;
}
.person-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  object-fit: cover;
  margin-right: 16px;
  background: #f0f0f0;
}
.person-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}
.person-name {
  font-size: 1.1rem;
  font-weight: 600;
  color: #222;
}
.person-date {
  font-size: 0.95rem;
  color: #888;
}
.menu-trigger-btn {
  width: 36px;
  height: 36px;
  border: none;
  border-radius: 50%;
  background: #f0f0f0;
  box-shadow: 0 1px 4px rgba(0,0,0,0.08);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  z-index: 2;
  transition: background 0.15s, box-shadow 0.15s;
  padding: 0;
  margin-left: 8px;
}
.menu-trigger-btn:hover, .menu-trigger-btn:focus {
  background: #e0e0e0;
  box-shadow: 0 2px 8px rgba(0,0,0,0.12);
}
.menu-dots {
  font-size: 22px;
  color: #555;
  line-height: 1;
}
.menu-dropdown {
  position: absolute;
  top: 48px;
  right: 12px;
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 6px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  z-index: 10;
  min-width: 120px;
}
.menu-item {
  padding: 10px 16px;
  cursor: pointer;
  transition: background 0.15s;
}
.menu-item:hover {
  background: #f5f5f5;
}
</style>

