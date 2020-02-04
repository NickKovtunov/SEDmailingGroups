<template>
  <v-container>
    <v-layout text-center
              wrap>
      <v-flex xs12>
        <v-icon style="font-size:120px; color: #1976d2;margin-top:50px;">mdi-email-mark-as-unread</v-icon>
      </v-flex>

      <v-flex mb-4>
        <h1 class="display-2 font-weight-bold mb-3">
          Группы рассылки
        </h1>
        <p class="subheading font-weight-regular">
          Справочник системы электронного документооборота
        </p>
      </v-flex>

      <v-flex mb-5
              xs12>
        <h2 class="headline font-weight-bold mb-3">Полезные ссылки</h2>

        <v-layout justify-center>
          <a v-for="(next, i) in whatsNext"
             :key="i"
             :href="next.href"
             class="subheading mx-3"
             target="_blank">
            {{ next.text }}
          </a>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  export default {
    data: () => ({
      whatsNext: [
        {
          text: 'СЭД (Справочники)',
          href: 'https://df.gduapp.ru/Settings',
        },
        {
          text: 'Портал',
          href: 'http://portal.gd-urengoy.gazprom.ru/default.aspx',
        },
      ],
    }),
  };
</script>
