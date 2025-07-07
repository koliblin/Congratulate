// stores/counter.js
import { defineStore } from 'pinia'
import * as api from '@/api.js'

export const usePersonsStore = defineStore('persons', {
    state: () => ({
        items: [],
        coming: [],
        loading: false,
        error: null
    }),
    actions: {
        async fetchAll() {
            this.loading = true
            try {
                this.items = await api.getAllPersons()
            } catch (e) {
                this.error = e
            } finally {
                this.loading = false
            }
        },
        async fetchComing() {
            this.loading = true
            try {
                this.coming = await api.getComingPersons()
            } catch (e) {
                this.error = e
            } finally {
                this.loading = false
            }
        },
        async create(person, file) {
            this.loading = true
            try {
                const created = await api.createPerson(person, file)
                this.items.push(created)
            } catch (e) {
                this.error = e
            } finally {
                this.loading = false
            }
        },
        async update(id, person, file) {
            this.loading = true
            try {
                await api.updatePerson(id, person, file)
                await this.fetchAll()
            } catch (e) {
                this.error = e
            } finally {
                this.loading = false
            }
        },
        async remove(id) {
            this.loading = true
            try {
                await api.deletePerson(id)
                this.items = this.items.filter(p => p.id !== id)
            } catch (e) {
                this.error = e
            } finally {
                this.loading = false
            }
        }
    }
})