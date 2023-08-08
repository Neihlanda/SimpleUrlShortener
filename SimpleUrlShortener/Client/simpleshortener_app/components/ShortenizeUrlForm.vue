<template>
  <div class="p-5 grid h-20 card bg-base-300 rounded-box place-items-center">
    <div class="join w-full sm:w-9/12">
      <input type="url" class="input input-bordered join-item w-full" placeholder="https://lurlaraccourcir.com"
        v-model="urlToShortenize" />
      <button class="btn join-item btn-primary" @click="shortenizeUrl">Raccourcir</button>
    </div>
  </div>
</template>

<script setup lang="ts">

const urlToShortenize = ref('');
function shortenizeUrl() {
//TODO: Call shortenize api
//TODO: Push history
}

</script>
