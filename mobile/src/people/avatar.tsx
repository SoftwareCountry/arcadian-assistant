import React, { Component } from 'react';
import { defaultAvatar } from './avatar-default';
import { View, Image, StyleSheet, ViewStyle, ImageStyle } from 'react-native';

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    outerFrame: {
        // Following attributes are default
        borderColor: '#2FAFCC',
        borderWidth: 2
    },
    image: {
        // Following attributes are default
        borderColor: '#fff',
        borderWidth: 2,
        flex: 1
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
    borderRadius?: number;
    size?: number;
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
    constructor(props: AvatarProps) {
        super(props);
        this.state = {};
    }

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

        const defaultStyle = StyleSheet.flatten(styles.default);

        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, {
            borderRadius: this.state.borderRadius || defaultStyle.borderRadius,
            width: this.state.size || defaultStyle.width,
            height: this.state.size || defaultStyle.height
        }]);

        const imgSize = (outerFrameFlattenStyle.width as number) - outerFrameFlattenStyle.borderWidth * 2;
        const imageFlattenStyle = StyleSheet.flatten([styles.image, {
            width: imgSize,
            height: imgSize,
            borderRadius: outerFrameFlattenStyle.borderRadius - outerFrameFlattenStyle.borderWidth * .5
        }]);

        return (
            <View onLayout={this.onLayout} style={styles.container}>
                <View style={outerFrameFlattenStyle}>
                    <Image source={{ uri: mimeType + photoBase64 }} style={imageFlattenStyle} />
                </View>
            </View>
        );
    }
}