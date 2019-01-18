import React, { Component } from 'react';
import { Image, ImageStyle, LayoutChangeEvent, StyleSheet, View, ViewStyle } from 'react-native';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Nullable } from 'types';
import FastImage from 'react-native-fast-image';
import { avatarStyles } from './styles';

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
        jwtToken: state.authentication && state.authentication.authInfo ? state.authentication.authInfo.jwtToken : null,
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

        const outerFrameFlattenStyle = StyleSheet.flatten([
            avatarStyles.outerFrame,
            {
                borderRadius: this.state.borderRadius,
                width: this.state.size,
                height: this.state.size
            },
            this.props.style,
            this.state.visible ? {} : { display: 'none' }
        ]);

        return (
            <View onLayout={this.onLayout} style={avatarStyles.container}>
                <View style={outerFrameFlattenStyle}>
                    {this.renderImage(outerFrameFlattenStyle)}
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private renderImage(containerStyle: ViewStyle) {
        const imgSize = (containerStyle.width as number) - containerStyle.borderWidth! * 2;
        const imageFlattenStyle = StyleSheet.flatten([
            avatarStyles.image,
            {
                width: imgSize,
                height: imgSize,
                borderRadius: imgSize * 0.5,
                borderWidth: containerStyle.borderWidth! * 2 //by design it seems to be twice thicker than container border
            },
            this.props.imageStyle
        ]) as ImageStyle;

        if (!this.props.photoUrl || this.state.loadingErrorOccurred) {
            const defaultImage = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;

            return <Image source={defaultImage} style={imageFlattenStyle}/>;
        }

        const headers = { 'Authorization': `Bearer ${this.props.jwtToken}` };

        return <FastImage style={imageFlattenStyle}
                          source={{ uri: this.props.photoUrl, headers: headers }} onLoadEnd={this.onImageLoaded}
                          onError={this.onImageLoadError}/>;
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
