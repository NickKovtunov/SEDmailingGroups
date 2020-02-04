<!--<template>
  <div>
    <div v-if="!pageLoaded" class="text-center">
      <h1><icon icon="spinner" pulse /></h1>
    </div>

    <div v-if="pageLoaded">
      <p v-if="mg.type == 0" class="main-title">
        Сведения о группе рассылки

        <v-menu bottom
                left
                transition="slide-y-transition"
                v-if="hasAdminRights">
          <template v-slot:activator="{ on }">
            <v-btn icon
                   v-on="on"
                   style="float: right;">
              <v-icon>mdi-dots-vertical</v-icon>
            </v-btn>
          </template>
          <v-list dense
                  nav>
            <v-list-item link
                         router
                         :to="`/edit/` + id"
                         class="menu-point">
              <v-list-item-icon>
                <v-icon>mdi-settings</v-icon>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>Редактировать</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>
      </p>
      <p v-if="mg.type == 1" class="main-title">
        Сведения о контейнере

        <v-menu bottom
                left
                transition="slide-y-transition"
                v-if="hasAdminRights">
          <template v-slot:activator="{ on }">
            <v-btn icon
                   v-on="on"
                   style="float: right;">
              <v-icon>mdi-dots-vertical</v-icon>
            </v-btn>
          </template>
          <v-list dense
                  nav>
            <v-list-item link
                         router
                         :to="`/edit/` + id"
                         class="menu-point">
              <v-list-item-icon>
                <v-icon>mdi-settings</v-icon>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>Редактировать</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>
      </p>
      <div class="form-wrapper clearfix" v-if="mg.name">
        <div class="label-wrap">
          <label for="name">Наименование</label>
        </div>
        <div class="field-wrap">
          <v-text-field v-model="mg.name"
                        id="name"
                        outlined
                        dense
                        filled
                        readonly></v-text-field>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.description">
        <div class="label-wrap">
          <label for="description">Описание</label>
        </div>
        <div class="field-wrap">
          <v-text-field v-model="mg.description"
                        id="description"
                        outlined
                        dense
                        filled
                        readonly></v-text-field>
        </div>
      </div>

      <div class="form-wrapper clearfix">
        <div class="label-wrap">
          <label for="type">Тип объекта</label>
        </div>
        <div class="field-wrap">
          <v-select id="type"
                    item-text="title"
                    item-value="value"
                    v-model="mg.type"
                    :items="types"
                    outlined
                    dense
                    filled
                    readonly
                    class="custom-disabled"></v-select>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && departments.length > 0">
        <div class="label-wrap">
          <label for="departments">Подразделения</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="departments"
                          item-text="fullTitle"
                          item-value="unitEmployeeDbId"
                          v-model="departments"
                          :items="departments"
                          outlined
                          dense
                          filled
                          chips
                          multiple
                          readonly
                          class="custom-disabled"></v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && users.length > 0">
        <div class="label-wrap">
          <label for="users">Пользователи</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="users"
                          item-text="fio"
                          item-value="userGuid"
                          v-model="users"
                          :items="users"
                          outlined
                          dense
                          filled
                          chips
                          multiple
                          readonly
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(1)"
                          class="custom-disabled"></v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && approves.length > 0">
        <div class="label-wrap">
          <label for="approves">Утверждающие</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="approves"
                          item-text="fio"
                          item-value="userGuid"
                          v-model="approves"
                          :items="approves"
                          outlined
                          dense
                          filled
                          chips
                          multiple
                          readonly
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(2)"
                          class="custom-disabled"></v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && admins.length > 0">
        <div class="label-wrap">
          <label for="admins">Администраторы</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="admins"
                          item-text="fio"
                          item-value="userGuid"
                          v-model="admins"
                          :items="admins"
                          outlined
                          dense
                          filled
                          chips
                          multiple
                          readonly
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(3)"
                          class="custom-disabled"></v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && exceptions.length > 0">
        <div class="label-wrap">
          <label for="exceptions">Исключения</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="exceptions"
                          item-text="fio"
                          item-value="userGuid"
                          v-model="exceptions"
                          :items="exceptions"
                          outlined
                          dense
                          filled
                          chips
                          multiple
                          readonly
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(4)"
                          class="custom-disabled"></v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && AD">
        <div class="label-wrap">
          <label for="AD">Группа Active Directory</label>
        </div>
        <div class="field-wrap">
          <v-text-field v-model="AD.Name + ` (` + AD.Mail + `)`"
                        id="AD"
                        outlined
                        dense
                        filled
                        readonly></v-text-field>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="mg.type == 0 && mg.isSync">
        <div class="label-wrap">
          <label>&nbsp;</label>
        </div>
        <div class="field-wrap">
          <v-checkbox v-model="mg.isSync" label="Синхронизирована с группой Active Directory" readonly></v-checkbox>
        </div>
      </div>
    </div>

    <v-dialog v-model="dialog"
              scrollable
              max-width="1200">
      <v-card>
        <v-card-title>
          {{ dialogTitle }}
          <v-spacer></v-spacer>
          <v-text-field v-model="tableSearch"
                        append-icon="mdi-magnify"
                        label="Поиск"
                        single-line
                        hide-details></v-text-field>
        </v-card-title>

        <v-card-text style="max-height: 500px;">
          <v-data-table :headers="tableHeaders"
                        :items="tablePeople"
                        :search="tableSearch"
                        :items-per-page="tablePeople.length"
                        hide-default-footer></v-data-table>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>

          <v-btn color="blue darken-1"
                 text
                 @click="dialog = false">
            Закрыть
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
  export default {
    data() {
      return {
        pageLoaded: false,
        id: this.$route.params.id,
        mg: null,
        AD: null,
        types: [
          {
            "value": 0,
            "title": "Группа рассылки",
          },
          {
            "value": 1,
            "title": "Контейнер",
          }
        ],
        users: [],
        approves: [],
        admins: [],
        exceptions: [],
        departments: [],

        dialog: false,
        dialogTitle: "",
        tableSearch: '',
        tableHeaders: [
          { text: 'ФИО', value: 'fio' },
          { text: 'Подразделение', value: 'department' },
          { text: 'Должность', value: 'position' },
        ],
        tablePeople: [],
      }
    },

    watch: {
      $route(toR, fromR) {
        this.id = toR.params['id']
        this.loadPage()
      },
    },
    methods: {
      async loadPage() {
        this.pageLoaded = false

        try {
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/show/get/` + this.id)
          this.mg = response.data

          let responseDepartments = await this.$http.get(this.$store.state.baseUrl + `/api/Departments/GetDepartments/` + this.id)
          this.departments = responseDepartments.data

          let responsePeople = await this.$http.get(this.$store.state.baseUrl + `/api/show/getPeople/` + this.id)
          this.users = []
          this.approves = []
          this.admins = []
          this.exceptions = []
          const self = this
          responsePeople.data.forEach(function(item) {
            switch (item.type) {
              case 1:
                self.users.push(item);
                break;
              case 2:
                self.approves.push(item);
                break;
              case 3:
                self.admins.push(item);
                break;
              case 4:
                self.exceptions.push(item);
                break;
            }
          });

          let responseAD = await this.$http.get(this.$store.state.baseUrl + `/api/ADGroup/Search?term=` + this.mg.aDgroup)
          this.AD = responseAD.data[0]

          this.pageLoaded = true
        } catch (err) {
          window.alert('Ошибка загрузки данных о группе рассылки')
          console.log(err)
        }

      },

      openPeopleTable(type) {
        this.dialogTitle = ""
        this.tablePeople = []
        const self = this
        switch (type) {
          case 1:
            self.dialogTitle = "Пользователи";
            self.tablePeople = self.users;
            break;
          case 2:
            self.dialogTitle = "Утверждающие";
            self.tablePeople = self.approves;
            break;
          case 3:
            self.dialogTitle = "Администраторы";
            self.tablePeople = self.admins;
            break;
          case 4:
            self.dialogTitle = "Исключения";
            self.tablePeople = self.exceptions;
            break;
        }
        this.dialog = true
      },
    },

    created() {
      this.loadPage()
    },

    computed: {
      hasAdminRights() {
        return this.$http.get(this.$store.state.baseUrl + `/api/values/HasAdminRights/?id=` + this.id)
      },
    }
  }
</script>-->
