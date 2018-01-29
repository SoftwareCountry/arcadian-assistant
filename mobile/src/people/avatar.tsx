import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle } from 'react-native';

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
    image:  {
        // Following attributes are default
        borderColor: '#fff',
        borderWidth: 2,
    }
});

export interface AvatarProps {
    mimeType: string;
    photoBase64: string;
    style: ViewStyle;
}

export class Avatar extends Component<AvatarProps> {
    private bordersWidth: 2;
    private borderRadius: number;
    
    public onLayout = (e: any) => {
        this.borderRadius = e.nativeEvent.layout.width * 0.5;
        this.setState({
          width: e.nativeEvent.layout.width,
          height: e.nativeEvent.layout.height,
        });
    }

    public render() {
        const { mimeType, photoBase64, style } = this.props;
        const outerFrameFlattenStyle = StyleSheet.flatten([styles.outerFrame, this.props.style, {borderRadius: this.borderRadius}]);
        const imageFlattenStyle = StyleSheet.flatten([this.props.style, styles.image, {width: (this.borderRadius * 2 - 2), height: (this.borderRadius * 2 - 2), borderRadius: (this.borderRadius - 2)}]);

        return (
            <View style={outerFrameFlattenStyle} onLayout={ this.onLayout }>
                <Image source={{uri: mimeType + photoBase64}} style={imageFlattenStyle} />
            </View>
        );
    }
}

