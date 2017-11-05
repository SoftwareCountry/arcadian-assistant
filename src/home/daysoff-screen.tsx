import React, { Component } from 'react';
import { View, Text, ScrollView, Button } from 'react-native';
import { NavigationScreenProps } from 'react-navigation';

export class DaysoffScreen extends Component<NavigationScreenProps<{}>> {
    public render() {
        return <ScrollView>
                <Text>This is not the screen you're looking for</Text>
                <Button title='Back' onPress={ () => this.props.navigation.goBack() } ></Button>
            </ScrollView>;
    }
}