import { LocaleHelper } from '@bryntum/scheduler';

const locale = {

  localeName: 'Ru',
  localeDesc: 'Русский',
  localeCode: 'ru',

  Column: {
    Staff: 'Персонал',
    'Task color': 'Цвет задачи'
  }

};

export default LocaleHelper.publishLocale(locale);
