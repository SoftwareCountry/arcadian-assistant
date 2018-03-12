import React, { Component } from 'react';
import { Animated, Easing, View, Image, Dimensions, StyleSheet } from 'react-native';
import { Avatar } from './avatar';

export interface Props {
    employeeName: string;
}

export class EmployeeCardWithAvatar extends Component<Props> {
    public state = {
        fadeInAnim: new Animated.Value(0),
        fadeOutAnim: new Animated.Value(1)
    };

    public revealNeighborsAvatars = () => {
        console.log('pop up neighbors');
    }

    public componentDidMount() {
        Animated.timing(
          this.state.fadeInAnim,
          {
            toValue: 1,
            duration: 2000,
          }
        ).start();

        Animated.timing(
            this.state.fadeOutAnim,
            {
              toValue: 0,
              duration: 1000,
            }
        ).start();
    }

    public render() {
        const { employeeName } = this.props;
        const { fadeInAnim, fadeOutAnim } = this.state;

        return (
            <Animated.View style={{ width: Dimensions.get('window').width, height: 100, justifyContent: 'space-between', opacity: fadeInAnim }}>
                <View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: -50 * 0.5 }}><Avatar style={{ width: 50, height: 50 }} /></View>
                <Avatar style={{ width: 150, height: 150 }} />
                <View style={{ position: 'absolute', top: (100 - 50) * 0.5, left: (Dimensions.get('window').width - 50 * 0.5 )}}><Avatar style={{ width: 50, height: 50 }} /></View>
            </Animated.View>
        );
    }
}