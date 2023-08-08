<template>
  <div>
    <RedirectToDestinationUrlLoader v-if="shortUrlDescriptionToRedirect !== undefined"
      :url-description="shortUrlDescriptionToRedirect" />
    <template v-else>
      <div class="flex flex-col w-full">
        <ShortenizeUrlForm />
        <div class="divider"></div>
        <component :is="isLoggedIn ? UrlShortenedHistory : SignInLogInForm" />
      </div>
    </template>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, useRoute } from '#imports';
import { ShortUrlDescription } from '../models/shorturldescription';
import { UrlShortenedHistory, SignInLogInForm } from '#components'

const route = useRoute();

const shortUrlDescriptionToRedirect = ref<ShortUrlDescription>();

const isLoggedIn = ref(false);

onMounted(async () => {
  if (route.params.id && route.params.id.length >= 0) {
    if (Array.isArray(route.params.id))
      loadUrlDescription(route.params.id[0]);
    else
      loadUrlDescription(route.params.id);
  }
});

async function loadUrlDescription(urlDescriptionId: string) {
  $fetch(
    `api/urlshortener/${urlDescriptionId}`
  ).then()
}
</script>
