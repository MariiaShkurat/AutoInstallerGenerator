<template>
    <div class="card glass">
        <h1 class="text-2xl font-semibold mb-4">Generated Installers</h1>

        <v-data-table v-if="list.length" class="table-dark" :headers="headers" :items="list">
            <template #item.builtAt="{ item }">
                {{ formatDate(item.builtAt) }}
            </template>

            <template #item.log="{ item }">
              <v-btn color="transparent" @click="openLog(item)" icon flat>
                <v-icon class="fa-solid fa-file" />
              </v-btn>
            </template>

            <template #item.download="{ item }">
                <v-btn color="transparent" :href="item.url" icon flat>
                    <v-icon class="fa-solid fa-download" />
                </v-btn>
            </template>
        </v-data-table>

        <v-alert v-else type="info" class="mt-4">No installers found.</v-alert>
    </div>

    <v-dialog v-model="logDialog" max-width="700">
      <v-card class="pa-4">
        <v-card-title class="text-h6">
          Build log — {{ selected?.projectName }}
        </v-card-title>

        <v-divider class="my-2"/>

        <v-card-text style="max-height: 60vh; overflow-y: auto; white-space: pre-wrap;">
          {{ selected?.log }}
        </v-card-text>

        <v-card-actions class="justify-end">
          <v-btn text color="primary" @click="logDialog = false">
            Close
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Installer {
  id: string
  projectName: string
  culture: string
  builtAt: string
  url: string
  log: string
}

const list = ref<Installer[]>([])
const logDialog  = ref(false)
const selected   = ref<Installer | null>(null)

const headers = [{
    title: 'Project',
    value: 'projectName'
  },
  {
    title: 'Generated at',
    value: 'builtAt',
    sortable: true
  },
  {
    title: 'Culture',
    value: 'culture'
  },
  {
    title: 'Log',
    value: 'log'
  },
  {
    title: 'Download',
    value: 'download',
    sortable: false
  },
]

function formatDate(dateIso: string) {
  const d = new Date(dateIso)
  return d.toLocaleString('uk-UA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit'
  })
}

function openLog(item: Installer) {
  selected.value = item
  logDialog.value = true
}

onMounted(async () => {
  try {
    const res  = await fetch('/api/installers')
    const data = await res.json()

    list.value = data.map((d: any) => ({
      ...d,
      url: `/api/installers/${d.id}/file`,
    }))
  } catch (err) {
    console.error('Error fetching installers:', err)
  }
})
</script>

<style scoped>
.table-dark {
    background-color: transparent;
    color: white;
}
</style>
