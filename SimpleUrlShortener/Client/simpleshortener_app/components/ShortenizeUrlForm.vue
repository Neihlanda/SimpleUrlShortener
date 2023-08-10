<template>
  <div class="p-5 grid h-20 card bg-base-300 rounded-box place-items-center">
    <div class="join w-full sm:w-9/12">
      <input type="url" class="input input-bordered join-item w-full" placeholder="https://www.lurlaraccourcir.com"
        v-model="urlToShortenize" />
      <button class="btn join-item btn-primary" @click="shortenizeUrl" :disabled="isFetching" >Raccourcir</button>
    </div>
  </div>

  <div v-if="shorturldescriptionCreated" class="p-5 grid card bg-base-300 rounded-box place-items-center">
    Résultat :
    <span>{{ shorturldescriptionCreated.scrappedTitle }}</span>
    <span>{{ shorturldescriptionCreated.scrappedDescription }}</span>
    <input type="url" disabled class="input input-bordered w-full" v-model="shorturldescriptionCreated.destinationUrl" />
    <div class="join w-full sm:w-9/12">
      <input type="url" disabled class="input input-bordered join-item w-full" v-model="shortenizedUrl" />
      <button class="btn join-item btn-primary btn-outline" @click="copyShortenizdUrl">
        Copier
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useToast } from 'vue-toast-notification';
import { ErrorDetails, ShortUrlDescription } from '../models';
const urlToShortenize = ref('');
const urlIsUniqueAccess = ref(false);
const shorturldescriptionCreated = ref<ShortUrlDescription>();
const shortenedUrlStore = useShortenedUrlStore();
const isFetching = computed(() => shortenedUrlStore.isFetching);
const $toast = useToast();

const shortenizedUrl = computed(() => {
  if (!shorturldescriptionCreated.value)
    return '';

  const url = new URL(window.location.origin)
  url.searchParams.append('id', shorturldescriptionCreated.value.id)
  return url.toString();
})

function shortenizeUrl() {
  if (isFetching.value) return;

  shortenedUrlStore
    .shortenizeUrl(urlToShortenize.value, urlIsUniqueAccess.value)
    .then((data) => {
      if(data)
        shorturldescriptionCreated.value = data;
    })
    .catch((error: ErrorDetails) => {
      error?.messages?.forEach(p =>
        $toast.error(p)
      )}
    );
}


function copyShortenizdUrl() {
  if (shortenizedUrl.value.length === 0)
    return;
  copyToClipBoard(shortenizedUrl.value).then(() =>
    $toast.success('URL copié au presse papier.')
  );
}


</script>
