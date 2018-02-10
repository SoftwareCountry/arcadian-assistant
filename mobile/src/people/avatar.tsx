import React, { Component } from 'react';
import { defaultAvatar } from './avatar-default';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    outerFrame: {
        // Following attributes are default
        borderColor: '#2FAFCC',
    },
    image: {
        // Following attributes are default
        borderColor: '#fff',
        flex: 1
    },
    default: {
        height: 200,
        width: 200,
        borderRadius: 200 * 0.5,
        borderWidth: 1
    }
});

export interface AvatarProps {
    mimeType?: string;
    photoBase64?: string;
    style?: ViewStyle;
    noInnerBorder?: boolean;
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

    public onLayout = (e: LayoutChangeEvent) => {
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
        const outerFrameBorderWidth = this.props.style ? this.props.style.borderWidth : defaultStyle.borderWidth;
        const imageBorderWidth = this.props.noInnerBorder ? 0 : outerFrameBorderWidth * 2;

        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, {
            borderRadius: this.state.borderRadius || defaultStyle.borderRadius, 
            borderWidth: outerFrameBorderWidth,
            width: this.state.size || defaultStyle.width,
            height: this.state.size || defaultStyle.height
        }]);

        const imgSize = (outerFrameFlattenStyle.width as number) - outerFrameFlattenStyle.borderWidth * 2;
        const imageFlattenStyle = StyleSheet.flatten([styles.image, {
            width: imgSize,
            height: imgSize,
            borderRadius: imgSize * 0.5,
            borderWidth: imageBorderWidth
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