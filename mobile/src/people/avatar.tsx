import React, { Component } from 'react';
import { defaultAvatar } from './avatar-default';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';

const styles = StyleSheet.create({
    outerFrame: {
        justifyContent: 'center',
        alignItems: 'center',
        // Following attributes are default
        borderColor: '#2FAFCC',
        borderWidth: 2,
        height: 200,
        width: 200,
        borderRadius: 200 * 0.5,
    },
    image: {
        // Following attributes are default
        borderColor: '#fff',
        borderWidth: 2
    },
    default: {
        height: 200,
        width: 200,
        borderRadius: 200 * 0.5
    }
});

export interface AvatarProps {
    mimeType?: string;
    photoBase64?: string;
    style?: ViewStyle;
}

interface AvatarState {
    borderRadius: number;
    size: number;
}

function validateMimeType(mime: string) {
    if (mime && mime.indexOf('data:') < 0) {
        mime = `data:${mime};`;
    }

    return mime;
}

function validateEncodedImage(data: string) {
    if (data && data.indexOf('base64') < 0) {
        data = `base64,${data}`;
    }

    return data;
}

export class Avatar extends Component<AvatarProps, AvatarState> {
    public state: AvatarState = {
        size: NaN,
        borderRadius: NaN
    };

    public onLayout = (e: any) => {
        let size = Math.min(e.nativeEvent.layout.width, e.nativeEvent.layout.height);
        this.setState({
            size: size,
            borderRadius: size * .5
        });
    }

    public render() {
        const mimeType = validateMimeType(this.props.mimeType || defaultAvatar.mimeType);
        const photoBase64 = validateEncodedImage(this.props.photoBase64 || defaultAvatar.base64);
        const style = this.props.style || styles.default;

        const elStyle = { borderRadius: this.state.borderRadius };
        if (isNaN(this.state.borderRadius)) {
            elStyle.borderRadius = parseFloat(style.width as any) * .5;
        }

        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, style, elStyle]);

        const imgSize = this.state.size - outerFrameFlattenStyle.borderWidth * 2;
        const imageFlattenStyle = StyleSheet.flatten([style, styles.image, {
            width: imgSize,
            height: imgSize,
            borderRadius: this.state.borderRadius - outerFrameFlattenStyle.borderWidth * .5
        }]);

        return (
            <View style={outerFrameFlattenStyle} onLayout={this.onLayout}>
                <Image source={{ uri: mimeType + photoBase64 }} style={imageFlattenStyle} />
            </View>
        );
    }
}