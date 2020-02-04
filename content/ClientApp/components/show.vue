<template>
  <div>
    <div v-if="!pageLoaded" class="text-center">
      <h1><icon icon="spinner" pulse /></h1>
    </div>

    <div v-if="pageLoaded">
      <p class="main-title">
        {{pageTitle}}

        <v-menu bottom
                left
                transition="slide-y-transition"
                v-if="hasAdminRights"
                min-width="200">
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

            <v-list-item v-if="!deleted"
                         link
                         class="menu-point"
                         @click="deleteObj()">
              <v-list-item-icon>
                <v-icon>mdi-delete</v-icon>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>Удалить</v-list-item-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item v-else
                         link
                         class="menu-point"
                         @click="restoreObj()">
              <v-list-item-icon>
                <v-icon>mdi-delete-restore</v-icon>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>Восстановить</v-list-item-title>
              </v-list-item-content>
            </v-list-item>

            <v-list-item link
                         class="menu-point"
                         @click="getHistory()">
              <v-list-item-icon>
                <v-icon>mdi-history</v-icon>
              </v-list-item-icon>

              <v-list-item-content>
                <v-list-item-title>История</v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </v-list>
        </v-menu>
      </p>

      <div class="form-wrapper clearfix" v-if="name">
        <div class="label-wrap">
          <label for="name">Наименование</label>
        </div>
        <div class="field-wrap">
          <v-text-field v-model="name"
                        id="name"
                        outlined
                        dense
                        background-color="white"
                        readonly></v-text-field>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="description">
        <div class="label-wrap">
          <label for="description">Описание</label>
        </div>
        <div class="field-wrap">
          <v-text-field v-model="description"
                        id="description"
                        outlined
                        dense
                        background-color="white"
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
                    v-model="type"
                    :items="types"
                    outlined
                    dense
                    background-color="white"
                    readonly
                    class="custom-disabled"></v-select>
        </div>
      </div>

      <div class="form-wrapper chip-wrapper clearfix" v-if="type == 0 && departments.length > 0">
        <div class="label-wrap">
          <label>Подразделения</label>
        </div>
        <div class="field-wrap">
          <div class="chips">
            <v-chip v-for="(d, index) in departments">
              {{d.fullTitle}}
            </v-chip>
          </div>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && selectedUsers.length > 0">
        <div class="label-wrap">
          <label for="users">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
                <v-icon v-on="on">mdi-information-outline</v-icon>
              </template>
              <span>Пользователи, которые входят в группу рассылки</span>
            </v-tooltip>
            Пользователи
          </label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="users"
                          v-model="selectedUsers"
                          :items="usersRequestResult"
                          outlined
                          dense
                          hide-no-data
                          item-text="fio"
                          item-value="Guid"
                          chips
                          multiple
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(1)"
                          return-object
                          background-color="white"
                          readonly
                          class="custom-disabled">
            <template v-slot:selection="data">
              <v-chip v-bind="data.attrs"
                      :input-value="data.selected"
                      @click="data.select">
                <v-avatar left v-if="data.item.incorrectInf">
                  <v-tooltip top>
                    <template v-slot:activator="{ on }">
                      <v-icon v-on="on">mdi-account-details</v-icon>
                    </template>
                    <span>Сведения о сотруднике неакутальны</span>
                  </v-tooltip>
                </v-avatar>
                {{ data.item.fio }}
              </v-chip>
            </template>
            <template v-slot:item="data">
              <v-list-item-content>
                <v-list-item-title v-html="data.item.fio"></v-list-item-title>
                <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
              </v-list-item-content>
            </template>
          </v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && selectedApproves.length > 0">
        <div class="label-wrap">
          <label for="approves">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
                <v-icon v-on="on">mdi-information-outline</v-icon>
              </template>
              <span>Пользователи, которым предоставлено право утверждения</span>
            </v-tooltip>
            Утверждающие
          </label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="approves"
                          v-model="selectedApproves"
                          :items="approvesRequestResult"
                          outlined
                          dense
                          hide-no-data
                          item-text="fio"
                          item-value="Guid"
                          chips
                          multiple
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(2)"
                          return-object
                          background-color="white"
                          readonly
                          class="custom-disabled">
            <template v-slot:selection="data">
              <v-chip v-bind="data.attrs"
                      :input-value="data.selected"
                      @click="data.select">
                <v-avatar left v-if="data.item.incorrectInf">
                  <v-tooltip top>
                    <template v-slot:activator="{ on }">
                      <v-icon v-on="on">mdi-account-details</v-icon>
                    </template>
                    <span>Сведения о сотруднике неакутальны</span>
                  </v-tooltip>
                </v-avatar>
                {{ data.item.fio }}
              </v-chip>
            </template>
            <template v-slot:item="data">
              <v-list-item-content>
                <v-list-item-title v-html="data.item.fio"></v-list-item-title>
                <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
              </v-list-item-content>
            </template>
          </v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && selectedAdmins.length > 0">
        <div class="label-wrap">
          <label for="admins">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
                <v-icon v-on="on">mdi-information-outline</v-icon>
              </template>
              <span>Пользователи, которым предоставляется право изменять данную группу рассылки и вложенные</span>
            </v-tooltip>
            Администраторы
          </label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="admins"
                          v-model="selectedAdmins"
                          :items="adminsRequestResult"
                          outlined
                          dense
                          hide-no-data
                          item-text="fio"
                          item-value="Guid"
                          chips
                          multiple
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(3)"
                          return-object
                          background-color="white"
                          readonly
                          class="custom-disabled">
            <template v-slot:selection="data">
              <v-chip v-bind="data.attrs"
                      :input-value="data.selected"
                      @click="data.select">
                <v-avatar left v-if="data.item.incorrectInf">
                  <v-tooltip top>
                    <template v-slot:activator="{ on }">
                      <v-icon v-on="on">mdi-account-details</v-icon>
                    </template>
                    <span>Сведения о сотруднике неакутальны</span>
                  </v-tooltip>
                </v-avatar>
                {{ data.item.fio }}
              </v-chip>
            </template>
            <template v-slot:item="data">
              <v-list-item-content>
                <v-list-item-title v-html="data.item.fio"></v-list-item-title>
                <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
              </v-list-item-content>
            </template>
          </v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && selectedExceptions.length > 0">
        <div class="label-wrap">
          <label for="exceptions">
            <v-tooltip left>
              <template v-slot:activator="{ on }">
                <v-icon v-on="on">mdi-information-outline</v-icon>
              </template>
              <span>Пользователи, которые исключены из группы рассылки</span>
            </v-tooltip>
            Исключения
          </label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="exceptions"
                          v-model="selectedExceptions"
                          :items="exceptionsRequestResult"
                          outlined
                          dense
                          hide-no-data
                          item-text="fio"
                          item-value="Guid"
                          chips
                          multiple
                          append-outer-icon="mdi-arrow-expand-all"
                          @click:append-outer="openPeopleTable(4)"
                          return-object
                          background-color="white"
                          readonly
                          class="custom-disabled">
            <template v-slot:selection="data">
              <v-chip v-bind="data.attrs"
                      :input-value="data.selected"
                      @click="data.select">
                <v-avatar left v-if="data.item.incorrectInf">
                  <v-tooltip top>
                    <template v-slot:activator="{ on }">
                      <v-icon v-on="on">mdi-account-details</v-icon>
                    </template>
                    <span>Сведения о сотруднике неакутальны</span>
                  </v-tooltip>
                </v-avatar>
                {{ data.item.fio }}
              </v-chip>
            </template>
            <template v-slot:item="data">
              <v-list-item-content>
                <v-list-item-title v-html="data.item.fio"></v-list-item-title>
                <v-list-item-subtitle>({{data.item.department}}, {{data.item.position}})</v-list-item-subtitle>
              </v-list-item-content>
            </template>
          </v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && ADgroup">
        <div class="label-wrap">
          <label for="AD">Группа Active Directory</label>
        </div>
        <div class="field-wrap">
          <v-autocomplete id="AD"
                          v-model="ADgroup"
                          :items="ADrequestResult"
                          outlined
                          dense
                          hide-no-data
                          item-text="Name"
                          item-value="Guid"
                          return-object
                          chips
                          background-color="white"
                          readonly
                          class="custom-disabled">
            <template v-slot:selection="data">
              <v-chip v-bind="data.attrs"
                      :input-value="data.selected"
                      @click="data.select">
                {{ data.item.Name }} ({{ data.item.Mail }})
              </v-chip>
            </template>
            <template v-slot:item="data">
              <v-list-item-content>
                <v-list-item-title v-html="data.item.Name"></v-list-item-title>
                <v-list-item-subtitle v-html="data.item.Mail"></v-list-item-subtitle>
              </v-list-item-content>
            </template>
          </v-autocomplete>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="type == 0 && isSync">
        <div class="label-wrap">
          <label>&nbsp;</label>
        </div>
        <div class="field-wrap">
          <v-checkbox v-model="isSync" label="Синхронизировать с группой Active Directory" readonly></v-checkbox>
        </div>
      </div>

      <div class="form-wrapper clearfix" v-if="showHistory">
        <div class="label-wrap">
          <label>История</label>
        </div>
        <div class="field-wrap">
          <div v-if="!historyLoaded" class="text-center">
            <h1><icon icon="spinner" pulse /></h1>
          </div>

          <v-simple-table style="background: transparent" v-if="historyLoaded">
            <template v-slot:default>
              <thead>
                <tr>
                  <th class="text-left">Дата</th>
                  <th class="text-left">Сотрудник</th>
                  <th class="text-left">Таблица</th>
                  <th class="text-left">Факт</th>
                  <th class="text-left">Объект</th>
                  <th class="text-left">ID объекта</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="item in history" :key="item.id">
                  <td>{{ item.date | dateFormat }}</td>
                  <td>{{ item.person | nameFormat }}</td>
                  <td>{{ item.place }}</td>
                  <td>{{ item.fact }}</td>
                  <td>{{ item.name }}</td>
                  <td>
                    <span v-if="item.place == `Unit`">{{item.depId}}</span>
                    <span v-if="item.place == `User`">{{item.userGuid}}</span>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
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
            <v-data-table v-if="tablePeople"
                          :headers="tableHeaders"
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
  </div>
</template>

<script>
  export default {
    data() {
      return {
        pageLoaded: false,
        id: this.$route.params.id,
        currentGuid: null,
        treeReloadProperty: false,
        parent: {
          "guid": null,
          "fullTitle": ""
        },
        parentDialog: false,

        departmentsDialog: false,
        departments: [],
        departmentsResult: [],
        departmentTreeReloadProperty: false,

        name: "",
        description: "",
        type: 0,
        deleted: false,
        notDeletedChildren: false,

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

        usersRequestResult: [],
        isLoadingUsers: false,
        selectedUsers: null,
        searchUsers: null,

        approvesRequestResult: [],
        isLoadingApproves: false,
        selectedApproves: null,
        searchApproves: null,

        adminsRequestResult: [],
        isLoadingAdmins: false,
        selectedAdmins: null,
        searchAdmins: null,

        exceptionsRequestResult: [],
        isLoadingExceptions: false,
        selectedExceptions: null,
        searchExceptions: null,

        AllSelectedPeople: [],

        ADrequestResult: [],
        isLoadingADgroups: false,
        ADgroup: null,
        searchADgroups: null,

        isSync: false,

        dialog: false,
        dialogTitle: "",
        tableSearch: '',
        tableHeaders: [
          { text: 'ФИО', value: 'fio' },
          { text: 'Подразделение', value: 'department' },
          { text: 'Должность', value: 'position' },
        ],
        tablePeople: [],

        history: [],
        showHistory: false,
        historyLoaded: false,
      }
    },

    methods: {
      async loadPage() {
        this.pageLoaded = false
        if (!this.hasAdminRights) {
          alert("У Вас нет прав на добавление группы рассылки")
          this.$router.push({ path: '/' })
        }

        try {
          this.history = []
          this.showHistory = false
          this.historyLoaded = false
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/show/get/` + this.id + `?datetime=` + new Date().getMilliseconds())
          this.name = response.data.name
          this.description = response.data.description
          this.type = response.data.type
          this.deleted = response.data.deleted
          this.notDeletedChildren = response.data.notDeletedChildren

          if (response.data.parentId) {
            this.parent.guid = response.data.parentId
            let responseParent = await this.$http.get(this.$store.state.baseUrl + `/api/values/GetFullParentName/?id=` + response.data.parentId + `&datetime=` + new Date().getMilliseconds())
            this.parent.fullTitle = responseParent.data
          }

          if (response.data.aDgroup) {
            let responseAD = await this.$http.get(this.$store.state.baseUrl + `/api/ADGroup/Search?term=` + response.data.aDgroup + `&datetime=` + new Date().getMilliseconds())
            this.ADrequestResult = responseAD.data
            this.ADgroup = responseAD.data[0]
          } else {
            this.ADgroup = null
          }

          this.isSync = response.data.isSync

          let responseDepartments = await this.$http.get(this.$store.state.baseUrl + `/api/Departments/GetDepartments/` + this.id + `?datetime=` + new Date().getMilliseconds())
          this.departments = responseDepartments.data

          let responsePeople = await this.$http.get(this.$store.state.baseUrl + `/api/show/getPeople/` + this.id + `?datetime=` + new Date().getMilliseconds())
          this.selectedUsers = []
          this.selectedApproves = []
          this.selectedAdmins = []
          this.selectedExceptions = []
          const self = this
          responsePeople.data.forEach(function (item) {
            switch (item.type) {
              case 1:
                self.usersRequestResult.push(item);
                self.selectedUsers.push(item);
                break;
              case 2:
                self.approvesRequestResult.push(item);
                self.selectedApproves.push(item);
                break;
              case 3:
                self.adminsRequestResult.push(item);
                self.selectedAdmins.push(item);
                break;
              case 4:
                self.exceptionsRequestResult.push(item);
                self.selectedExceptions.push(item);
                break;
            }
          });

          this.pageLoaded = true
        } catch (err) {
          window.alert('Ошибка загрузки данных о группе рассылки')
          console.log(err)
          this.$router.push('/')
        }
      },

      getAllSelectedPeople(selected, mailingGroupGuid, type) {
        if (selected && selected.length > 0) {
          const self = this
          selected.forEach(function (item, i, arr) {
            let u = {
              UserGuid: item.Guid,
              MailingGroupId: mailingGroupGuid,
              Type: type,
              FIO: item.fio,
              Position: item.position,
              Department: item.department,
              IncorrectInf: false,
              Deleted: false,
            }
            self.AllSelectedPeople.push(u)
          });
        }
      },

      async createObject() {
        if (this.name == "") {
          alert('Поле "Название" обязательно для заполнения')
        } else {
          try {
            const self = this
            let obj = {}
            if (this.type) {
              obj = {
                ParentId: this.parent.guid,
                Name: this.name,
                Description: this.description,
                Type: this.type,
              }
            } else {
              obj = {
                ParentId: this.parent.guid,
                Name: this.name,
                Description: this.description,
                Type: this.type,
                ADgroup: this.ADgroup ? this.ADgroup.Guid : null,
                IsSync: this.isSync,
              }
            }
            this.$http.post(this.$store.state.baseUrl + `/api/add/AddObj`, obj)
              .then(response => {
                this.currentGuid = response.data.id
                if (!this.type) {
                  if (this.departments.length > 0) {
                    this.departments.forEach(function (item, i, arr) {
                      let d = {
                        MailingGroupId: response.data.id,
                        UnitEmployeeDbId: item.id[0].toString(),
                        Deleted: false,
                      }
                      self.departmentsResult.push(d)
                    });
                    this.$http.post(this.$store.state.baseUrl + `/api/add/AddUnits`, this.departmentsResult)
                  }

                  this.getAllSelectedPeople(this.selectedUsers, response.data.id, 1)
                  this.getAllSelectedPeople(this.selectedApproves, response.data.id, 2)
                  this.getAllSelectedPeople(this.selectedAdmins, response.data.id, 3)
                  this.getAllSelectedPeople(this.selectedExceptions, response.data.id, 4)

                  if (this.AllSelectedPeople && this.AllSelectedPeople.length > 0) {
                    this.$http.post(this.$store.state.baseUrl + `/api/add/AddUsers`, this.AllSelectedPeople)
                  }
                }
              })
              .then(response => {
                alert("Объект успешно добавлен")
                this.$router.push('/show/' + this.currentGuid)
              })
          } catch (err) {
            window.alert('Ошибка добавления объекта')
            console.log(err)
          }
        }
      },

      async changeNode(id) {
        if (id.length > 0) {
          this.parentDialog = false
          this.parent.guid = id[0]
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/values/GetFullParentName/?id=` + this.parent.guid + `&datetime=` + new Date().getMilliseconds())
          this.parent.fullTitle = response.data
        } else {
          this.parent.guid = null,
            this.parent.fullTitle = ""
        }
      },

      removeParent() {
        this.treeReloadProperty = !this.treeReloadProperty
      },

      openParentModel() {
        this.parentDialog = true
      },

      openDepartmentsModel() {
        this.departmentsDialog = true
      },

      async changeDepartmentsNode(id) {
        if (id.length > 0) {
          this.departmentsDialog = false
          let d = []
          d.id = id
          let response = await this.$http.get(this.$store.state.baseUrl + `/api/departmentsTree/GetFullDepartmentName/?id=` + d.id + `&datetime=` + new Date().getMilliseconds())
          d.fullTitle = response.data
          this.departments.push(d)
          this.departmentTreeReloadProperty = !this.departmentTreeReloadProperty
        } else { }
      },

      removeDepartment(index) {
        this.departments.splice(index, 1)
      },

      removePeople(place, item) {
        const index = place.findIndex(user => user["Guid"] === item.Guid)
        if (index >= 0) place.splice(index, 1)
      },

      openPeopleTable(type) {
        this.dialogTitle = ""
        this.tablePeople = []
        const self = this
        switch (type) {
          case 1:
            self.dialogTitle = "Пользователи";
            self.tablePeople = self.selectedUsers;
            break;
          case 2:
            self.dialogTitle = "Утверждающие";
            self.tablePeople = self.selectedApproves;
            break;
          case 3:
            self.dialogTitle = "Администраторы";
            self.tablePeople = self.selectedAdmins;
            break;
          case 4:
            self.dialogTitle = "Исключения";
            self.tablePeople = self.selectedExceptions;
            break;
        }
        this.dialog = true
      },

      deleteObj() {
        if (this.hasAdminRights) {
          var qustion = "Удалить объект?"
          if (this.notDeletedChildren) {
            qustion = "У объекта есть неудалённые дочерние элементы, которые будут удалены. Продолжить?"
          }

          if (confirm(qustion)) {
            try {
              let obj = {
                Id: this.id,
              }
              this.$http.post(this.$store.state.baseUrl + `/api/values/DeleteObj`, obj)
                .then(response => {
                  alert("Объект успешно удален")
                  this.$router.push('/')
                })
            } catch (err) {
              window.alert('Ошибка удаления объекта')
              console.log(err)
            }
          }
        } else {
          alert("У Вас нет прав на удаление")
        }
      },

      restoreObj(){
        if (this.hasAdminRights) {
          if (confirm("Восстановить объект?")) {
            try {
              let obj = {
                Id: this.id,
              }

              this.$http.post(this.$store.state.baseUrl + `/api/values/CanRestoreObj`, obj)
                .then(response => {
                  if (response.data) {
                    this.$http.post(this.$store.state.baseUrl + `/api/values/RestoreObj`, obj)
                      .then(response => {
                        alert("Объект успешно восстановлен")
                        this.$store.state.treeReloadProperty = !this.$store.state.treeReloadProperty
                        this.loadPage()
                      })
                  } else {
                    alert("Для восстановления объекта необходимо восстановить его родителей")
                  }
                })
            } catch (err) {
              window.alert('Ошибка восстановления объекта')
              console.log(err)
            }
          }
        } else {
          alert("У Вас нет прав на восстановление")
        }
      },

      async getHistory() {
        this.showHistory = true
        let response = await this.$http.get(this.$store.state.baseUrl + `/api/departments/GetHistory/?id=` + this.id + `&datetime=` + new Date().getMilliseconds())
        this.history = response.data
        this.historyLoaded = true
      },
    },

    created() {
      this.loadPage()
    },

    watch: {
      $route(toR, fromR) {
        this.id = toR.params['id']
        this.loadPage()
      },

      selectedUsers() {
        this.searchUsers = null
      },

      selectedApproves() {
        this.searchApproves = null
      },

      selectedAdmins() {
        this.searchAdmins = null
      },

      selectedExceptions() {
        this.searchExceptions = null
      },

      ADgroup() {
        this.searchADgroups = null
      },

      searchUsers(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        if (this.isLoadingUsers) return

        this.isLoadingUsers = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.usersRequestResult = this.usersRequestResult.concat(res)
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingUsers = false))
      },

      searchApproves(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        if (this.isLoadingApproves) return

        this.isLoadingApproves = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.approvesRequestResult = this.approvesRequestResult.concat(res)
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingApproves = false))
      },

      searchAdmins(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        if (this.isLoadingAdmins) return

        this.isLoadingAdmins = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.adminsRequestResult = this.adminsRequestResult.concat(res)
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingAdmins = false))
      },

      searchExceptions(val) {
        if (!val) {
          return
        }
        val = encodeURI(val)

        if (this.isLoadingExceptions) return

        this.isLoadingExceptions = true
        fetch(this.$store.state.baseUrl + `/api/ADUser/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.exceptionsRequestResult = this.exceptionsRequestResult.concat(res)
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingExceptions = false))
      },

      searchADgroups(val) {
        if (!val) {
          return
        }

        val = encodeURI(val)

        if (this.isLoadingADgroups) return

        this.isLoadingADgroups = true
        fetch(this.$store.state.baseUrl + `/api/ADGroup/Search?term=` + val)

          .then(res => res.json())
          .then(res => {
            this.ADrequestResult = res
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => (this.isLoadingADgroups = false))
      },
    },

    computed: {
      hasAdminRights() {
        return this.$http.get(this.$store.state.baseUrl + `/api/values/HasAdminRights/?id=` + this.id + `&datetime=` + new Date().getMilliseconds())
      },

      pageTitle() {
        var title = ""

        if (this.deleted) {
          if (this.type) {
            title = "Сведения об удаленном контейнере"
          } else {
            title = "Сведения об удаленной группе рассылки"
          }
        } else {
          if (this.type) {
            title = "Сведения о контейнере"
          } else {
            title = "Сведения о группе рассылки"
          }
        }

        return title
      }
    }
  }
</script>
