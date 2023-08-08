<template>
  <div class="hero min-h-screen bg-base-200">
    <div class="hero-content text-center w-9/12">
      <div class="max-w-xl">
        <h1 class="text-5xl font-bold">Redirection en cours...</h1>

        <div class="divider"></div>

        <div class="flex flex-col">
          <div class="grid h-40 min-h-full flex-grow card bg-base-300 rounded-box place-items-center">
            <span>{{ urlDescription.scrappedTitle }}</span>
            <span>{{ urlDescription.scrappedDescription }}</span>

            <div v-if="abortRedirectRequested">
              Redirection automatique annulée.
            </div>
            <div v-else>
              Vous allez être redirigé automatiquement
              dans <span class="countdown">
                <span :style="`--value:${remainingTimerHitBeforeRedirect};`"></span>
              </span> seconde(s)
            </div>
          </div>

          <div class="divider">OU</div>

          <div class="grid flex-grow card bg-base-300 rounded-box place-items-center p-4">
            Continuer directement vers la destination :
            <a class="link break-all" :href="urlDescription.destinationUrl">{{
              urlDescription.destinationUrl
            }}</a>
          </div>
          <div class="divider"></div>

        </div>
        <button class="btn btn-primary btn-wide" @click="abortRedirectRequested = true">Annuler la redirection
          automatique</button>
      </div>
    </div>



    <div class="divider"></div>

  </div>
</template>

<script setup lang="ts">
import { ref, watch } from '#imports';
import { ShortUrlDescription } from '../models/shorturldescription';

declare type RedirectToDestinationUrlLoaderProps = {
  urlDescription: ShortUrlDescription;
};
const defaultTimerHit = 20;
const abortRedirectRequested = ref(false);
const remainingTimerHitBeforeRedirect = ref(defaultTimerHit)
const props = defineProps<RedirectToDestinationUrlLoaderProps>();

watch(
  () => props.urlDescription,
  () => redirectToDescription()
);

onMounted(redirectToDescription);

function redirectToDescription() {
  if (!props.urlDescription || props.urlDescription.destinationUrl?.length == 0) return;

  abortRedirectRequested.value = false;
  remainingTimerHitBeforeRedirect.value = defaultTimerHit;

  const oneSecond = 1000;
  var remainingHit = setInterval(() => {
    if (remainingTimerHitBeforeRedirect.value == 0) {
      clearInterval(remainingHit);
      window.location.replace(props.urlDescription.destinationUrl);
    } else {
      remainingTimerHitBeforeRedirect.value--;
    }

    if (abortRedirectRequested.value)
      clearInterval(remainingHit);

  }, oneSecond);

};

</script>
