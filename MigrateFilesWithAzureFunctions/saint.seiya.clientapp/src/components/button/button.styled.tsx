import styled, { ThemeProvider } from 'styled-components'
import React, { FunctionComponent } from 'react'
import { theme } from '../../assets'
import { ButtonProps } from './button.styled.props'

const StyledButton = styled.button`
	font-size: 1em;
	margin: 1em;
	padding: 0.25em 1em;
	border-radius: 3px;
	cursor: pointer;
	color: ${props => props.theme.main};
	border: 2px solid ${props => props.theme.main};
`

export const Button: FunctionComponent<ButtonProps> = props => {
	return (
		<>
			<ThemeProvider theme={theme}>
				<StyledButton onClick={props.onClick}>{props.text}</StyledButton>
			</ThemeProvider>
		</>
	)
}