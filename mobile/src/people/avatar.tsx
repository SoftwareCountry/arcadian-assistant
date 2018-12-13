import React, { Component } from 'react';
import { Image, ImageStyle, LayoutChangeEvent, StyleSheet, View, ViewStyle } from 'react-native';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Nullable } from 'types';
import FastImage from 'react-native-fast-image';

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
        borderWidth: 1
    },
    image: {
        borderColor: '#fff',
        flex: 1
    }
});

export interface AvatarOwnProps {
    photoUrl?: string;
    style?: ViewStyle;
    imageStyle?: ViewStyle;
    useDefaultForEmployeesList?: boolean;
}

export interface AvatarReduxProps {
    jwtToken: Nullable<string>;
}

function mapStateToProps(state: AppState): AvatarReduxProps {
    return {
        jwtToken: state.authentication && state.authentication.authInfo ? state.authentication.authInfo.jwtToken : null,
    };
}

// tslint:disable-next-line:no-var-requires
const employeesListAvatarRect = require('./employeesListAvatarRect.png');
// tslint:disable-next-line:no-var-requires
const arcadiaIcon = require('./arcadia-icon.png');

interface AvatarState {
    borderRadius?: number;
    size?: number;
    visible: boolean;
}

type AvatarProps = AvatarOwnProps & AvatarReduxProps;

class AvatarImpl extends Component<AvatarProps, AvatarState> {
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
    };

    public render() {
        if (!this.state.size) {
            return <View onLayout={this.onLayout} style={styles.container}/>;
        }

        const outerFrameFlattenStyle = StyleSheet.flatten([
            styles.outerFrame,
            {
                borderRadius: this.state.borderRadius,
                width: this.state.size,
                height: this.state.size
            },
            this.props.style,
            this.state.visible ? {} : { display: 'none' }
        ]);

        return (
            <View onLayout={this.onLayout} style={styles.container}>
                <View style={outerFrameFlattenStyle}>
                    {this.renderImage(outerFrameFlattenStyle)}
                </View>
            </View>
        );
    }

    private renderImage(containerStyle: ViewStyle) {
        const imgSize = (containerStyle.width as number) - containerStyle.borderWidth! * 2;
        const imageFlattenStyle = StyleSheet.flatten([
            styles.image,
            {
                width: imgSize,
                height: imgSize,
                borderRadius: imgSize * 0.5,
                borderWidth: containerStyle.borderWidth! * 2 //by design it seems to be twice thicker than container border
            },
            this.props.imageStyle
        ]);

        if (!(this.props.photoUrl && this.props.jwtToken)) {
            const defaultImage = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;

            return <Image source={defaultImage} style={imageFlattenStyle as ImageStyle}/>;
        }

        const headers = { 'Authorization': `Bearer ${this.props.jwtToken}` };

        return <FastImage style={imageFlattenStyle as ImageStyle}
                          source={{ uri: this.props.photoUrl, headers: headers }}/>;
    }
}

export const Avatar = connect<AvatarReduxProps, {}, AvatarOwnProps, AppState>(mapStateToProps)(AvatarImpl);
