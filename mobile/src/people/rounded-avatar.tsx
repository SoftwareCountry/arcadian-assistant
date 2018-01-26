import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle } from 'react-native';

const styles = StyleSheet.create({
    outerFrame: {
        justifyContent: 'center', 
        alignItems: 'center'
    }
});

export interface RoundedAvatarProps {
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

export class RoundedAvatar extends Component<RoundedAvatarProps> {
    public render() {
        const { mimeType, photoBase64, outerFrameStyle, imageStyle } = this.props;
        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, outerFrameStyle]);

        return (
            <View style={outerFrameFlattenStyle}>
                <Image source={{uri: mimeType + photoBase64}} style={imageStyle} />
            </View>
        );
    }
}

