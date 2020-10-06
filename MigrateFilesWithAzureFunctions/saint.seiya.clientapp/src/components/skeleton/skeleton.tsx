import React from 'react'
import { Shimmer, ShimmerElementType } from 'office-ui-fabric-react/lib/Shimmer'
import { mergeStyles } from 'office-ui-fabric-react/lib/Styling'
import { Fabric } from 'office-ui-fabric-react/lib/Fabric'
import { SkeletonProps } from './skeleton.props'

export const Skeleton = (props: SkeletonProps) => {
	const wrapperClass = mergeStyles({
		padding: 2,
		selectors: {
			'& > .ms-Shimmer-container': {
				margin: '10px 0',
			},
		},
	})

	return (
		<Fabric className={wrapperClass}>
			<Shimmer
				shimmerElements={[
					{ type: ShimmerElementType.circle },
					{ type: ShimmerElementType.gap, width: '2%' },
					{ type: ShimmerElementType.line },
				]}
			/>
		</Fabric>
	)
}
