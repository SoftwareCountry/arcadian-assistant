import React, { Component } from 'react';
import { View, Image, StyleSheet, ViewStyle, ImageStyle, LayoutChangeEvent } from 'react-native';
import { Photo } from '../../reducers/organization/employee.model';
import { PhotoMap } from '../../reducers/organization/employees.reducer';
import { loadPhoto } from '../../reducers/organization/organization.action';
import { AppState } from '../../reducers/app.reducer';
import { connect, Dispatch } from 'react-redux';
import { styles } from './avatar-styles';

export interface AvatarProps {
    id?: string;
    style?: ViewStyle;
    imageStyle?: ViewStyle;
    useDefaultForEmployeesList?: boolean;
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

interface AvatarStateProps {
    photoById: PhotoMap;
}

interface AvatarDispatchProps {
    loadPhoto: (id: string) => void;
}

const mapStateToProps = (state: AppState): AvatarStateProps => ({
    photoById: state.organization.photoById,
});

const mapDispatchToProps = (dispatch: Dispatch<any>): AvatarDispatchProps => ({
    loadPhoto: (id: string) => dispatch(loadPhoto(id)),
});

class AvatarImpl extends Component<AvatarProps & AvatarStateProps & AvatarDispatchProps, AvatarState> {
    constructor(props: AvatarProps & AvatarStateProps & AvatarDispatchProps) {
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
        let image = this.props.useDefaultForEmployeesList ? employeesListAvatarRect : arcadiaIcon;

        if (this.props.id) {
            const photo = this.props.photoById.get(this.props.id);
            if (!photo) {
                this.props.loadPhoto(this.props.id);
            }
            const mimeType = this.validateMimeType(photo);
            const photoBase64 = this.validateEncodedImage(photo);

            if (mimeType && photoBase64) {
                image = { uri: mimeType + photoBase64 };
            }
        }

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

export const Avatar = connect(mapStateToProps, mapDispatchToProps)(AvatarImpl);