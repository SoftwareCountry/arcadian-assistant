import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';
import { Photo } from '../reducers/organization/employee.model';

const styles = StyleSheet.create({
    container: {
        width: '100%',
        height: '100%',
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center'
    },
    outerFrame: {
        borderColor: '#2FAFCC',
        borderWidth: 2
    },
    image: {
        borderColor: '#fff',
        borderWidth: 2,
        flex: 1
    }
});

export interface AvatarProps {
    photo?: Photo;
    style?: ViewStyle;
}

interface AvatarState {
    borderRadius?: number;
    size?: number;
    visible: boolean;
}

export class Avatar extends Component<AvatarProps, AvatarState> {
    constructor(props: AvatarProps) {
        super(props);
        this.state = {
            visible: false
        };
    }

    public onLayout = (e: LayoutChangeEvent) => {
        let size = Math.min(e.nativeEvent.layout.width, e.nativeEvent.layout.height);
        this.setState({
            size: size,
            borderRadius: size * .5,
            visible: true
        });
    }

    public render() {
        const mimeType = this.validateMimeType(this.props.photo);
        const photoBase64 = this.validateEncodedImage(this.props.photo);

        const image = !mimeType || !photoBase64 ? require('../../src/people/userpic.png') : { uri: mimeType + photoBase64 };

        const outerFrameFlattenStyle = StyleSheet.flatten([
            styles.outerFrame,
            {
                borderRadius: this.state.borderRadius,
                width: this.state.size,
                height: this.state.size
            },
            this.state.visible ?
                {}
                : { display: 'none' }
        ]);

        const imgSize = (outerFrameFlattenStyle.width as number) - outerFrameFlattenStyle.borderWidth * 2;
        const imageFlattenStyle = StyleSheet.flatten([
            styles.image,
            {
                width: imgSize,
                height: imgSize,
                borderRadius: outerFrameFlattenStyle.borderRadius - outerFrameFlattenStyle.borderWidth * .5
            }]);

        return (
            <View onLayout={this.onLayout} style={styles.container}>
                <View style={outerFrameFlattenStyle}>
                    <Image source={image} style={imageFlattenStyle} />
                </View>
            </View>
        );
    }

    private validateMimeType(photo: Photo) {
        let mime = photo ? photo.mimeType : null;
        if (mime && mime.indexOf('data:') < 0) {
            mime = `data:${mime};`;
        }

        return mime;
    }

    private validateEncodedImage(photo: Photo) {
        let data = photo ? photo.base64 : null;
        if (data && data.indexOf('base64') < 0) {
            data = `base64,${data}`;
        }

        return data;
    }
}