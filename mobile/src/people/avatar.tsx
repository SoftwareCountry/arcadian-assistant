import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';
import { Optional } from 'types';

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
    photoUrl : Optional<string>;
    style?: ViewStyle;
    imageStyle?: ViewStyle;
    useDefaultForEmployeesList?: boolean;
}

export interface AvatarReduxProps {
    jwtToken: Optional<string>;
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
            return <View onLayout={this.onLayout} style={styles.container}></View>;
        }

        const defaultPhoto = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;

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

        const imgSize = (outerFrameFlattenStyle.width as number) - outerFrameFlattenStyle.borderWidth! * 2;
        const imageFlattenStyle = StyleSheet.flatten([
            styles.image,
            {
                width: imgSize,
                height: imgSize,
                borderRadius: imgSize * 0.5,
                borderWidth: outerFrameFlattenStyle.borderWidth! * 2 //by design it seems to be twice thicker than container border
            },
            this.props.imageStyle
        ]);

        const image = this.props.photoUrl && this.props.jwtToken
            ?
            {
                uri: this.props.photoUrl,
                headers: {
                    'Authorization': `Bearer ${this.props.jwtToken}`
                }
            }
            : defaultPhoto;

        return (
            <View onLayout={this.onLayout} style={styles.container}>
                <View style={outerFrameFlattenStyle}>
                    <Image source={image} defaultSource={defaultPhoto} style={imageFlattenStyle as ImageStyle} />
                </View>
            </View>
        );
    }
}

export const Avatar = connect<AvatarReduxProps, {}, AvatarOwnProps>(mapStateToProps)(AvatarImpl);
