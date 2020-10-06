import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import common_es from '../assets/translations/es/common.json';
import common_en from '../assets/translations/en/common.json';
import { store } from '../store';
import { actionsDef } from '../actions/actionsDef';

i18n.use(LanguageDetector)
	.use(initReactI18next)
	.init({
		whitelist: ['en', 'es'],
		load: 'languageOnly',
		fallbackLng: 'es',
		defaultNS: 'common',
		interpolation: {
			escapeValue: false,
			format: function (value, format, lng) {
				if (format === 'intlDate') return Intl.DateTimeFormat(lng).format(value);
				if (format === 'intlNumber') return Intl.NumberFormat(lng).format(value);
				return value;
			},
		},
		resources: {
			en: {
				common: common_en, 
			},
			es: {
				common: common_es, 
			},
		},
		detection: {
			order: ['querystring', 'cookie', 'localStorage', 'navigator'],
			lookupQuerystring: 'lng',
			lookupCookie: 'i18next',
			lookupLocalStorage: 'i18nextLng',
			caches: ['localStorage', 'cookie'],
		},
	});

const whitelist: string[] = ['en', 'es'];
const defaultLng: string = whitelist[1];
let selectedLng: string = i18n.language !== null ? i18n.language : defaultLng;

if (selectedLng.length >= 2) {
	selectedLng = selectedLng.split('-')[0];

	if (whitelist.some((lng: string) => lng === selectedLng)) {
		i18n.changeLanguage(selectedLng);
	} else {
		i18n.changeLanguage(defaultLng);
	}
} else {
	i18n.changeLanguage(defaultLng);
}

i18n.on('languageChanged', (newlocale) => {
	store.dispatch({
		type: actionsDef.i18n.CHANGE_LANGUAGE,
		payload: newlocale,
	});
});

export default i18n;
