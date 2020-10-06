import React from 'react'
import { NavLink } from 'react-router-dom'
import { routeUrls } from '../../consts/routes'

export const Sidebar = () => (
	<nav>
		<ul>
			<li>
				<NavLink
					exact
					activeStyle={{
						fontWeight: 'bold',
						color: 'red',
					}}
					to={routeUrls.home}
				>
					Home
				</NavLink>
			</li>
		</ul>
	</nav>
)
