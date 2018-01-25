import React, { Component } from 'react';
import { View, Image } from 'react-native';

export interface Props {
    photoBase64: string;
    outerBorderColor: string;
    // Inner border color could be same as superview's background color 
    // or some other color - refer to design.
    innerBorderColor: string;
    avatarSize: number;
    outerBorderWidth: number;
    innerBorderWidth: number;
}

export class RoundedAvatar extends Component<Props> {
    public render() {
        const { outerBorderColor, innerBorderColor, photoBase64, avatarSize, outerBorderWidth, innerBorderWidth } = this.props;
        const imageSize = avatarSize - outerBorderWidth;

        return (
            <View style={{ height: avatarSize, width: avatarSize, justifyContent: 'center', alignItems: 'center', borderRadius: avatarSize * 0.5, borderColor: outerBorderColor, borderWidth: outerBorderWidth }}>
                <Image source={{uri: photoBase64}} style={{ height: imageSize, width: imageSize, borderRadius: imageSize * 0.5, borderColor: innerBorderColor, borderWidth: innerBorderWidth }} />
            </View>
        );
    }
}
