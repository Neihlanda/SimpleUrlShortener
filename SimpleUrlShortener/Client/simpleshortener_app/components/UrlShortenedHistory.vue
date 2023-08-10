<template>
  <div v-if="isFetching" class="flex flex-col justify-center items-center">
    <span class="loading loading-spinner text-primary "></span>
  </div>
  <table class="table table-xs" v-else>
    <thead>
      <tr>
        <th>Lien</th>
        <th>Titre</th>
        <th>Description</th>
        <th>Nombre d'utilisation</th>
        <th>Expire le</th>
        <th></th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="shortUrl in urlHistory" :key="shortUrl.id">
        <td> {{ shortUrl.destinationUrl  }}</td>
        <td> {{ shortUrl.scrappedTitle  }}</td>
        <td> {{ shortUrl.scrappedDescription }}</td>
        <td> {{ shortUrl.accessCount}}</td>
        <td> {{ shortUrl.expiredOn }}</td>
        <td>
          <btn class="btn btn-primary btn-outline" @click="copyShortenizdUrl(shortUrl.id)">Copier le lien</btn>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script setup lang="ts">
import { useToast } from 'vue-toast-notification';
const shortenedUrlStore = useShortenedUrlStore();
const urlHistory = computed(() => shortenedUrlStore.history);
const isFetching = computed(() => shortenedUrlStore.isFetching);

const $toast = useToast();

onMounted(() => {
  shortenedUrlStore.fetchHistory();
});


function copyShortenizdUrl(id: string) {
  if (!id || id.length === 0)
    return;

  const url = new URL(window.location.origin)
  url.searchParams.append('id', id)
  copyToClipBoard(url.toString()).then(() =>
    $toast.success('URL copié au presse papier.')
  );
}


</script>
