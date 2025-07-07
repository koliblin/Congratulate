import { createRouter, createWebHistory } from 'vue-router'

import CreateView from "@/views/CreateView.vue";
import HomeView from "@/views/HomeView.vue";

const routes = [
    { path: '/', component: HomeView},
    { path: '/create', component: CreateView},
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router