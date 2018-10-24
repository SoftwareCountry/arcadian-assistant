import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';
import { Photo } from '../reducers/organization/employee.model';
import { connect } from 'react-redux';
import { AppState } from '../reducers/app.reducer';

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
    photo?: Photo;
    photoUrl? : string;
    style?: ViewStyle;
    imageStyle?: ViewStyle;
    useDefaultForEmployeesList?: boolean;
}

export interface AvatarReduxProps {
    jwtToken: string;
}

function mapStateToProps(state: AppState): AvatarReduxProps {
    return {
        jwtToken: state.authentication.authInfo.jwtToken
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
    }

    public render() {
        const mimeType = this.validateMimeType(this.props.photo);
        const photoBase64 = this.validateEncodedImage(this.props.photo);

        const defaultPhoto = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;
        const image = !mimeType || !photoBase64 ? defaultPhoto : { uri: mimeType + photoBase64 };

        const outerFrameFlattenStyle = StyleSheet.flatten([
            styles.outerFrame,
            {
                borderRadius: this.state.borderRadius,
                width: this.state.size,
                height: this.state.size
            },
            this.props.style,
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
                borderRadius: imgSize * 0.5,
                borderWidth: outerFrameFlattenStyle.borderWidth * 2 //by design it seems to be twice thicker than container border
            },
            this.props.imageStyle
        ]);

        const sourceObj = {
            uri: 'https://picjumbo.com/wp-content/uploads/man-in-a-hoodie-standing-still-against-crowds-on-charles-bridge-prague_free_stock_photos_picjumbo_DSC00938-2210x1473.jpg',
            headers: {
                'Authorization': `Bearer ${this.props.jwtToken}`
            }
        };

        return (
            <View onLayout={this.onLayout} style={styles.container}>
                <View style={outerFrameFlattenStyle}>
                    <Image source={sourceObj} style={imageFlattenStyle} />
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

export const Avatar = connect<AvatarReduxProps, {}, AvatarOwnProps>(mapStateToProps)(AvatarImpl);