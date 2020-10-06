import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import common_es from "./es/common.json";
import common_en from "./en/common.json";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    whitelist: ['en', 'es'],
    load: 'languageOnly',
    fallbackLng: 'en',
    resources: {
        'en': {
            common: common_en
        },
        'es': {
            common: common_es
        }
    },
    detection: {
        order: ['querystring', 'cookie', 'localStorage', 'navigator'],
        lookupQuerystring: 'lng',
        lookupCookie: 'i18next',
        lookupLocalStorage: 'i18nextLng',
        caches: ['localStorage', 'cookie']
    }
});

let whitelist: string[] = ['en', 'es'];
const defaultLng: string = whitelist[0];
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

export default i18n;