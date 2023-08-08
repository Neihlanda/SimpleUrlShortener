<template>
  <div></div>
</template>

<script setup lang="ts">
import { ShortUrlDescription } from 'models';


const urlHistory = ref<ShortUrlDescription[]>([]);

onMounted(() => {
  //TODO: load url history
});


</script>
