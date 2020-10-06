import React from 'react'
import { useTranslation } from 'react-i18next'
import { Dropdown, IDropdownOption } from 'office-ui-fabric-react/lib/Dropdown'

export const LanguageSelector = () => {
	const { i18n } = useTranslation()

	const onChangeLanguage = (event: React.FormEvent<HTMLDivElement>, option?: IDropdownOption): void => {
		i18n.changeLanguage(option ? option.key.toString() : 'en')
	}

	return (
		<div>
			<Dropdown
				styles={{ dropdown: { width: 300, float: 'right' } }}
				onChange={onChangeLanguage}
				options={[
					{ key: 'en', text: 'English', selected: true },
					{ key: 'es', text: 'Spanish' },
				]}
			/>
		</div>
	)
}
