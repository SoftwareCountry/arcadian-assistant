import React, { Component } from 'react';
import { Image, ImageStyle, LayoutChangeEvent, StyleSheet, View, ViewStyle } from 'react-native';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Nullable } from 'types';
import FastImage from 'react-native-fast-image';
import { avatarStyles } from './styles';
import Style from '../layout/style';

//============================================================================
export interface AvatarOwnProps {
    photoUrl?: string;
    style?: ViewStyle;
    imageStyle?: ViewStyle;
    useDefaultForEmployeesList?: boolean;
}

//============================================================================
export interface AvatarReduxProps {
    jwtToken: Nullable<string>;
}

//----------------------------------------------------------------------------
function mapStateToProps(state: AppState): AvatarReduxProps {
    return {
        jwtToken: state.authentication && state.authentication.jwtToken ? state.authentication.jwtToken.value : null,
    };
}

// tslint:disable-next-line:no-var-requires
const employeesListAvatarRect = require('./employeesListAvatarRect.png');
// tslint:disable-next-line:no-var-requires
const arcadiaIcon = require('./arcadia-icon.png');

//============================================================================
interface AvatarState {
    borderRadius?: number;
    size?: number;
    visible: boolean;
    loadingErrorOccurred: boolean;
}

//----------------------------------------------------------------------------
type AvatarProps = AvatarOwnProps & AvatarReduxProps;

//============================================================================
class AvatarImpl extends Component<AvatarProps, AvatarState> {

    //----------------------------------------------------------------------------
    constructor(props: AvatarProps) {
        super(props);
        this.state = {
            visible: false,
            loadingErrorOccurred: false,
        };
    }

    //----------------------------------------------------------------------------
    public onLayout = (e: LayoutChangeEvent) => {
        let size = Math.min(e.nativeEvent.layout.width, e.nativeEvent.layout.height);
        this.setState({
            size: size,
            borderRadius: size * .5,
            visible: true
        });
    };

    //----------------------------------------------------------------------------
    public render() {

        if (!this.state.size) {
            return <View onLayout={this.onLayout} style={avatarStyles.container}/>;
        }

        const outerFrameStyle = StyleSheet.flatten([
            avatarStyles.outerFrame,
            {
                borderRadius: this.state.borderRadius,
                width: this.state.size,
                height: this.state.size
            },
            this.props.style,
            this.state.visible ? {} : { display: 'none' }
        ]);

        const image = this.renderImage(outerFrameStyle);

        return (
            <View onLayout={this.onLayout}
                  style={avatarStyles.container}>
                <View style={outerFrameStyle}>
                    {image}
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private renderImage(outerFrameStyle: ViewStyle) {

        const imgSize = (outerFrameStyle.width as number) - outerFrameStyle.borderWidth! * 2;

        let windowBorderWidth = outerFrameStyle.borderWidth! * 1.5;
        if (this.props.imageStyle && this.props.imageStyle.borderWidth !== undefined) {
            windowBorderWidth = this.props.imageStyle.borderWidth;
        }

        const imageStyle = StyleSheet.flatten([
            avatarStyles.image,
            {
                width: imgSize,
                height: imgSize,
                borderRadius: imgSize * 0.5,
            },
            this.props.imageStyle
        ]) as ImageStyle;

        if (!this.props.photoUrl || this.state.loadingErrorOccurred) {
            const defaultImage = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;

            const defaultImageStyle = StyleSheet.flatten([
                imageStyle,
                {
                    borderWidth: windowBorderWidth,
                },
            ]) as ImageStyle;

            return <Image source={defaultImage} style={defaultImageStyle}/>;
        }

        const windowStyle: ViewStyle = {
            backgroundColor: 'transparent',
            borderColor: imageStyle.borderColor,
            width: imageStyle.width,
            height: imageStyle.height,
            borderRadius: imageStyle.borderRadius,
            borderWidth: windowBorderWidth,
        };

        const headers = { 'Authorization': `Bearer ${this.props.jwtToken}` };

        return (
            <FastImage style={imageStyle}
                       source={{ uri: this.props.photoUrl, headers: headers }}
                       onLoadEnd={this.onImageLoaded}
                       onError={this.onImageLoadError}>
                <View style={windowStyle}/>
            </FastImage>
        );
    }

    //----------------------------------------------------------------------------
    private onImageLoaded = () => {
        this.setState({
            ...this.state,
            loadingErrorOccurred: false,
        });
    };

    //----------------------------------------------------------------------------
    private onImageLoadError = () => {
        this.setState({
            ...this.state,
            loadingErrorOccurred: true,
        });
    };
}

//----------------------------------------------------------------------------
export const Avatar = connect<AvatarReduxProps, {}, AvatarOwnProps, AppState>(mapStateToProps)(AvatarImpl);
