import React from 'react';
import { View, Image } from 'react-native';
import { splashScreenStyles } from './styles';

export class SplashScreen extends React.Component<{}> {

    public render() {
        return (
            <View style={splashScreenStyles.container}>
                <View style={splashScreenStyles.imageContainer} >
                    <Image source={require('./arcadia-logo.png')}  />
                </View>
            </View>
        );
    }
}
