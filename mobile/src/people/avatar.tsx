import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle } from 'react-native';

const styles = StyleSheet.create({
    outerFrame: {
        justifyContent: 'center', 
        alignItems: 'center',
        // Following attributes are default and could be changed via component props
        height: 156,
        width: 156,
        borderRadius: 156 * 0.5,
        borderColor: '#2FAFCC',
        borderWidth: 2
    },
    image:  {
        // Following attributes are default and could be changed via component props
        height: 154,
        width: 154,
        borderRadius: 154 * 0.5,
        borderColor: '#fff',
        borderWidth: 2
    }
});

export interface AvatarProps {
    mimeType: string;
    photoBase64: string;
    // These attributes should be passed in following style props:
    // height, width, borderRadius, borderColor, borderWidth
    //
    // Border radius is used for circle mask effect and should be 50% of width/height
    //
    // Image border color could be same as superview's background color 
    // or some other color - refer to design.
    outerFrameStyle: ViewStyle;
    imageStyle: ImageStyle;
}

export class Avatar extends Component<AvatarProps> {
    public render() {
        const { mimeType, photoBase64, outerFrameStyle, imageStyle } = this.props;
        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, outerFrameStyle]);
        const imageFlattenStyle = StyleSheet.flatten([styles.image, imageStyle]);

        return (
            <View style={outerFrameFlattenStyle}>
                <Image source={{uri: mimeType + photoBase64}} style={imageFlattenStyle} />
            </View>
        );
    }
}

