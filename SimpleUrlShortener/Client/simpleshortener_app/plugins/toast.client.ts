import { useToast } from 'vue-toast-notification';

export default defineNuxtPlugin(() => {
  const toast = useToast();
  return {
    provide: {
      toast,
    },
  };
});
