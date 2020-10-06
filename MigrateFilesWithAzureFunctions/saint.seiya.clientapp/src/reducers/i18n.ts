import { actionsDef } from '../actions/actionsDef';

export const i18nReducer = (state: string = '', action: any) => {
	switch (action.type) {
		case actionsDef.i18n.CHANGE_LANGUAGE:
			return handleChangeLanguageCompleted(state, action.payload);
	}

	return state;
};

const handleChangeLanguageCompleted = (state: string, payload: string): string => {
	return payload;
};
