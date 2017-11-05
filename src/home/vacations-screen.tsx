import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps } from 'react-navigation';

export class VacationsScreen extends Component<NavigationScreenProps<{}>> {
    public render() {
        return <ScrollView>
                <Text>Si!</Text>
                <Button title='Back' onPress={ () => this.props.navigation.goBack() } ></Button>
            </ScrollView>;
    }
}